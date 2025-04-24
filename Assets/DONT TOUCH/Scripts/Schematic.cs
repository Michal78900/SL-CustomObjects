using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#pragma warning disable CS0618

[ExecuteInEditMode]
public class Schematic : SchematicBlock
{
    public override BlockType BlockType => BlockType.Schematic;

    [Tooltip("Allows non-uniform risky scaling on empty objects when enabled.")]
    [SerializeField]
    private bool AllowRiskyScaling = false;

    public void CompileSchematic()
    {
        if (!ProcessEmptyObjectsScaling())
        {
            Debug.LogError("<color=red>Failed to process empty objects scaling. Please check the console for details.</color>");
            return;
        }

        string parentDirectoryPath = Directory.Exists(Config.ExportPath)
            ? Config.ExportPath
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "MapEditorReborn_CompiledSchematics");

        string schematicDirectoryPath = Path.Combine(parentDirectoryPath, name);

        if (!Directory.Exists(parentDirectoryPath))
            Directory.CreateDirectory(parentDirectoryPath);

        if (Directory.Exists(schematicDirectoryPath))
            DeleteDirectory(schematicDirectoryPath);

        if (File.Exists($"{schematicDirectoryPath}.zip"))
            File.Delete($"{schematicDirectoryPath}.zip");

        Directory.CreateDirectory(schematicDirectoryPath);

        int rootObjectId = transform.GetInstanceID();
        /*
        SchematicObjectDataList blockList = new SchematicObjectDataList(rootObjectId);
        Dictionary<int, SerializableRigidbody> rigidbodyDictionary = new Dictionary<int, SerializableRigidbody>();
        List<SerializableTeleport> teleports = new List<SerializableTeleport>();
        */
        BlockList.RootObjectId = rootObjectId;
        BlockList.Blocks.Clear();
        RigidbodyDictionary.Clear();
        Teleports.Clear();

        if (TryGetComponent(out Rigidbody rigidbody))
            RigidbodyDictionary.Add(rootObjectId, new SerializableRigidbody(rigidbody));

        // transform.localScale = Vector3.one;

        foreach (Transform obj in GetComponentsInChildren<Transform>())
        {
            if (obj.CompareTag("EditorOnly") || obj == transform)
                continue;

            int objectId = obj.transform.GetInstanceID();

            SchematicBlockData block = new SchematicBlockData
            {
                Name = obj.name,
                ObjectId = objectId,
                ParentId = obj.parent.GetInstanceID(),
                Position = Quaternion.Euler(obj.parent.eulerAngles) * obj.localPosition,
            };

            if (obj.TryGetComponent(out SchematicBlock schematicBlock))
            {
                if (!schematicBlock.Compile(block, this))
                    continue;
            }
            else
            {
                // Light
                if (obj.TryGetComponent(out Light lightComponent))
                {
                    block.BlockType = BlockType.Light;
                    block.Properties = new Dictionary<string, object>
                    {
                        { "Color", ColorUtility.ToHtmlStringRGBA(lightComponent.color) },
                        { "Intensity", lightComponent.intensity },
                        { "Range", lightComponent.range },
                        { "Shadows", lightComponent.shadows != LightShadows.None },
                    };
                }
                else // Empty transform
                {
                    // Keep it
                    block.BlockType = BlockType.Empty;
                    block.Rotation = obj.localEulerAngles;
                }
            }

            if (obj.TryGetComponent(out Animator animator) && animator.runtimeAnimatorController != null)
            {
                RuntimeAnimatorController runtimeAnimatorController = animator.runtimeAnimatorController;
                block.AnimatorName = runtimeAnimatorController.name;

                BuildPipeline.BuildAssetBundle(runtimeAnimatorController,
                    runtimeAnimatorController.animationClips,
                    Path.Combine(schematicDirectoryPath, runtimeAnimatorController.name),
                    AssetBundleBuildOptions, EditorUserBuildSettings.activeBuildTarget);
            }

            if (obj.TryGetComponent(out rigidbody))
                RigidbodyDictionary.Add(objectId, new SerializableRigidbody(rigidbody));

            BlockList.Blocks.Add(block);
        }

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{name}.json"),
            JsonConvert.SerializeObject(BlockList, Formatting.Indented,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

        if (RigidbodyDictionary.Count > 0)
            File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{name}-Rigidbodies.json"),
                JsonConvert.SerializeObject(RigidbodyDictionary, Formatting.Indented,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

        if (Teleports.Count > 0)
            File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{name}-Teleports.json"),
                JsonConvert.SerializeObject(Teleports, Formatting.Indented,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

        if (Config.ZipCompiledSchematics)
        {
            System.IO.Compression.ZipFile.CreateFromDirectory(schematicDirectoryPath, $"{schematicDirectoryPath}.zip",
                System.IO.Compression.CompressionLevel.Optimal, true);
            Directory.Delete(schematicDirectoryPath, true);
        }

        Debug.Log($"<color=#00FF00><b>{name}</b> has been successfully compiled!</color>");
    }

    private bool ProcessEmptyObjectsScaling()
    {
        var proportional = new List<Transform>();
        var risky = new List<(Transform obj, Vector3 scale)>();

        foreach (Transform obj in GetComponentsInChildren<Transform>())
        {
            if (obj == transform) continue;

            if (!IsEmptyObject(obj)) continue;

            Vector3 scale = obj.localScale;

            if (scale == Vector3.one) continue;

            if (IsProportionalScaling(scale))
                proportional.Add(obj);
            else
                risky.Add((obj, scale));
        }

        if (!AllowRiskyScaling && risky.Count > 0)
        {
            foreach (var (obj, scale) in risky)
            {
                string path = GetTransformPath(obj);
                Debug.LogError(
                    $"<color=red>Non-uniform scaling detected on empty object <b>'{path}'</b>: {scale}. " +
                    "Please ensure uniform scaling (x = y = z) or enable AllowRiskyScaling.</color>");
            }
            return false;
        }

        if (AllowRiskyScaling)
        {
            if (risky.Count > 0)
            {
                foreach (var (obj, scale) in risky)
                {
                    string path = GetTransformPath(obj);
                    Debug.Log(
                        $"Risky scaling allowed on empty object <b>'{path}'</b>: {scale}. ");
                }
            }
            else
            {
                Debug.Log(
                    "<color=yellow>No risky empty objects detected. " +
                    "You can disable AllowRiskyScaling for stricter validation.</color>");
            }
        }

        var toProcess = new List<Transform>(proportional);
        if (AllowRiskyScaling)
            toProcess.AddRange(risky.ConvertAll(r => r.obj));

        if (toProcess.Count > 0)
        {
#if UNITY_EDITOR
            Undo.RecordObjects(
                toProcess.ConvertAll(t => (UnityEngine.Object)t).ToArray(),
                "Apply Scaling to Empty Objects");
#endif
            foreach (var obj in toProcess)
            {
                var children = obj.GetComponentsInChildren<Transform>()
                                  .Where(t => t != obj && !IsEmptyObject(t))
                                  .ToList();

                var originalParents = new Dictionary<Transform, Transform>();
                foreach (var child in children)
                {
                    originalParents[child] = child.parent;
                    child.SetParent(null);
                }
                foreach (var child in children)
                {
                    child.SetParent(originalParents[child]);
                }
            }

            Debug.Log(
                $"<color=#00FF00>Processed {toProcess.Count} empty objects " +
                $"{(AllowRiskyScaling ? "(including risky) " : "")}successfully.</color>");
        }

        return true;
    }

    private string GetTransformPath(Transform tr)
    {
        string path = tr.name;
        Transform parent = tr.parent;
        while (parent != null && parent != transform)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
    }

    private bool IsEmptyObject(Transform obj)
    {
        Component[] components = obj.GetComponents<Component>();
        if (components.Length > 1)
        {
            if (components.Length == 2 && obj.TryGetComponent(out SchematicBlock _))
                return true;

            return false;
        }
        return true;
    }

    private bool IsProportionalScaling(Vector3 scale)
    {
        const float epsilon = 0.00001f;
        return Mathf.Abs(scale.x - scale.y) < epsilon &&
               Mathf.Abs(scale.y - scale.z) < epsilon;
    }

    public void Update()
    {
        if (transform.localScale != Vector3.one)
        {
            transform.localScale = Vector3.one;
            Debug.LogError("<color=red>Do not change the scale of the root object!</color>");
        }

        if (name.Contains(" "))
        {
            name = name.Replace(" ", string.Empty);
            Debug.LogError("<color=red>Schematic name cannot contain spaces!</color>");
        }
    }

    // This is only used in nested schematics (schematics inside other schematics)
    public override bool Compile(SchematicBlockData block, Schematic _)
    {
        block.Rotation = transform.localEulerAngles;

        block.BlockType = BlockType.Schematic;
        block.Properties = new Dictionary<string, object>
        {
            { "SchematicName", name }
        };

        return false;
    }

    private static void DeleteDirectory(string path)
    {
        string[] files = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(path, false);
    }

    internal readonly SchematicObjectDataList BlockList = new SchematicObjectDataList();
    internal readonly Dictionary<int, SerializableRigidbody> RigidbodyDictionary = new Dictionary<int, SerializableRigidbody>();
    internal readonly List<SerializableTeleport> Teleports = new List<SerializableTeleport>();

    private static BuildAssetBundleOptions AssetBundleBuildOptions => BuildAssetBundleOptions.ChunkBasedCompression |
                                                                      BuildAssetBundleOptions.ForceRebuildAssetBundle |
                                                                      BuildAssetBundleOptions.StrictMode;

    private static readonly Config Config = SchematicManager.Config;
}
