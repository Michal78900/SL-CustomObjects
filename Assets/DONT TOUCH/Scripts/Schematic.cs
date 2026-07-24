using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#pragma warning disable CS0618

[ExecuteInEditMode]
public class Schematic : SchematicBlock
{
    public override BlockType BlockType => BlockType.Schematic;

    public void CompileSchematic()
    {
        if (TryGetComponent(out Animator rootAnimator))
        {
            Debug.LogWarning("[MER] UYARI: Animator bileşeni Kök (Root) nesne üzerinde bulundu! Kök nesneler şematik bloğu olmadığı için animasyon derlenemez. Lütfen Animator bileşenini altındaki bir boş objeye (Empty block) taşıyın.");
        }

        SetupOutput(out string schematicDirectoryPath);

        int rootObjectId = transform.GetInstanceID();
        BlockList.RootObjectId = rootObjectId;
        BlockList.Blocks.Clear();
        RigidbodyDictionary.Clear();
        Teleports.Clear();

        if (TryGetComponent(out Rigidbody rigidbody))
            RigidbodyDictionary.Add(rootObjectId, new SerializableRigidbody(rigidbody));

        foreach (SchematicBlock block in GetComponentsInChildren<SchematicBlock>())
        {
            if (block.CompareTag("EditorOnly") || block == this)
                continue;

            SchematicBlockData data = new();
            block.Compile(data);

            if (block.TryGetComponent(out Animator animator) && animator.runtimeAnimatorController != null)
            {
                RuntimeAnimatorController runtimeAnimatorController = animator.runtimeAnimatorController;
                data.AnimatorName = runtimeAnimatorController.name;

                BuildPipeline.BuildAssetBundle(runtimeAnimatorController,
                    runtimeAnimatorController.animationClips,
                    Path.Combine(schematicDirectoryPath, runtimeAnimatorController.name),
                    AssetBundleBuildOptions, EditorUserBuildSettings.activeBuildTarget);
            }

            if (block.TryGetComponent(out rigidbody))
                RigidbodyDictionary.Add(block.transform.GetInstanceID(), new SerializableRigidbody(rigidbody));

            BlockList.Blocks.Add(data);
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
    public override void Compile(SchematicBlockData block)
    {
        return;
        block.Rotation = transform.localEulerAngles;

        block.BlockType = BlockType.Schematic;
        block.Properties = new Dictionary<string, object>
        {
            { "SchematicName", name }
        };

        // return false;
    }

    private void SetupOutput(out string schematicDirectoryPath)
    {
        string parentDirectoryPath = Directory.Exists(Config.ExportPath) ? Config.ExportPath : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MapEditorReborn_CompiledSchematics");
        schematicDirectoryPath = Path.Combine(parentDirectoryPath, name);

        if (!Directory.Exists(parentDirectoryPath))
            Directory.CreateDirectory(parentDirectoryPath);

        if (Directory.Exists(schematicDirectoryPath))
            DeleteDirectory(schematicDirectoryPath);

        if (File.Exists($"{schematicDirectoryPath}.zip"))
            File.Delete($"{schematicDirectoryPath}.zip");

        Directory.CreateDirectory(schematicDirectoryPath);
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