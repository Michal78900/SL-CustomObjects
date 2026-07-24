using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Decompiler
{
    public class SchematicBuilder : MonoBehaviour
    {
        public bool TryGetBlockFromType(BlockType blockType, out SchematicBlock schematicBlock) => Dict.TryGetValue(blockType, out schematicBlock);

        private readonly Dictionary<BlockType, SchematicBlock> Dict = new();

        public SchematicBuilder Init()
        {
            Dict.Add(BlockType.Empty, gameObject.AddComponent<EmptyComponent>());
            Dict.Add(BlockType.Primitive, gameObject.AddComponent<PrimitiveComponent>());
            Dict.Add(BlockType.Light, gameObject.AddComponent<LightComponent>());
            Dict.Add(BlockType.Pickup, gameObject.AddComponent<PickupComponent>());
            Dict.Add(BlockType.Workstation, gameObject.AddComponent<WorkstationComponent>());
            Dict.Add(BlockType.Teleport, gameObject.AddComponent<TeleportComponent>());
            Dict.Add(BlockType.Locker, gameObject.AddComponent<LockerComponent>());
            Dict.Add(BlockType.Text, gameObject.AddComponent<TextComponent>());
            Dict.Add(BlockType.Interactable, gameObject.AddComponent<InteractableComponent>());
            Dict.Add(BlockType.Waypoint, gameObject.AddComponent<WaypointComponent>());

            return this;
		}
	}
    
    private static SchematicBuilder _schematicBuilder;

    [MenuItem("SchematicManager/Import Schematic/JSON")]
    private static void ImportSchematicJson()
    {
        string importPath = SchematicManager.Config.ExportPath;
        if (!Directory.Exists(importPath))
            Directory.CreateDirectory(importPath);

        string jsonFilePath = EditorUtility.OpenFilePanelWithFilters("Select json with the schemaitc", importPath, new string[] { "Schematic", "json" });
        if (string.IsNullOrEmpty(jsonFilePath))
        {
            Debug.LogError("Invalid schematic file. Path is empty.");
            return;
        }

        _schematicDirectoryPath = null;
        _schematicName = Path.GetFileNameWithoutExtension(jsonFilePath);
        _schematicData = JsonConvert.DeserializeObject<SchematicObjectDataList>(File.ReadAllText(jsonFilePath));

        PortBack();
    }

    [MenuItem("SchematicManager/Import Schematic/Folder")]
    private static void ImportSchematicFolder()
    {
        string importPath = SchematicManager.Config.ExportPath;
        if (!Directory.Exists(importPath))
            Directory.CreateDirectory(importPath);

        _schematicDirectoryPath = EditorUtility.OpenFolderPanel("Select folder with the schematic", importPath, "");
        if (string.IsNullOrEmpty(_schematicDirectoryPath))
        {
            Debug.LogError("Invalid schematic directory. Path is empty.");
            return;
        }

        _schematicName = Path.GetFileNameWithoutExtension(_schematicDirectoryPath);
        string jsonFilePath = Path.Combine(_schematicDirectoryPath, $"{_schematicName}.json");
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError("No json file found in the schematic directory!");
            return;
        }

        _schematicData = JsonConvert.DeserializeObject<SchematicObjectDataList>(File.ReadAllText(jsonFilePath));

        PortBack();
    }

    private static void PortBack()
    {
        _rootTransform = new GameObject(_schematicName).AddComponent<Schematic>().transform;
        _objectFromId = new Dictionary<int, Transform>(_schematicData.Blocks.Count + 1)
        {
            { _schematicData.RootObjectId, _rootTransform },
        };

        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
        Debug.Log("<color=#FFFF00>Importing schematic...</color>");

        _schematicBuilder = new GameObject("SchematicBuilder").AddComponent<SchematicBuilder>().Init();

        CreateRecursiveFromID(_schematicData.RootObjectId, _schematicData.Blocks, _rootTransform);

        if (_schematicDirectoryPath != null)
        {
            // CreateTeleporters();
            AddRigidbodies();
        }

        Debug.Log($"<color=#00FF00>Successfully imported <b>{_schematicName}</b> schematic in {stopwatch.ElapsedMilliseconds} ms!</color>");
        NullifyFields();

		Object.DestroyImmediate(_schematicBuilder.gameObject);
    }

    private static void CreateRecursiveFromID(int id, List<SchematicBlockData> blocks, Transform parentGameObject)
    {
        Transform childGameObjectTransform = CreateObject(blocks.Find(c => c.ObjectId == id), parentGameObject) ?? _rootTransform; // Create the object first before creating children.
        int[] parentSchematics = blocks.Where(bl => bl.BlockType == BlockType.Schematic).Select(bl => bl.ObjectId).ToArray();

        // Gets all the ObjectIds of all the schematic blocks inside "blocks" argument.
        foreach (SchematicBlockData block in blocks.FindAll(c => c.ParentId == id))
        {
            if (parentSchematics.Contains(block.ParentId)) // The block is a child of some schematic inside "parentSchematics" array, therefore it will be skipped to avoid spawning it and its children twice.
                continue;

            CreateRecursiveFromID(block.ObjectId, blocks, childGameObjectTransform); // The child now becomes the parent
        }
    }

    private static Transform CreateObject(SchematicBlockData block, Transform rootObject)
    {
        if (block == null)
            return null;

        GameObject gameObject = null;
		if (_schematicBuilder.TryGetBlockFromType(block.BlockType, out SchematicBlock schematicBlock))
		{
			schematicBlock.Decompile(ref gameObject, block, rootObject);
			_objectFromId.Add(block.ObjectId, gameObject.transform);
		}

		if (_schematicDirectoryPath != null && TryGetAnimatorController(block.AnimatorName, out RuntimeAnimatorController animatorController))
            gameObject.AddComponent<Animator>().runtimeAnimatorController = animatorController;

        return gameObject.transform;
    }

    private static bool TryGetAnimatorController(string animatorName, out RuntimeAnimatorController animatorController)
    {
        animatorController = null;

        if (!string.IsNullOrEmpty(animatorName))
        {
            Object animatorObject = AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault(x => x.mainAsset.name == animatorName)?.LoadAllAssets().First(x => x is RuntimeAnimatorController);

            if (animatorObject == null)
            {
                string path = Path.Combine(_schematicDirectoryPath, animatorName);

                if (!File.Exists(path))
                    return false;

                animatorObject = AssetBundle.LoadFromFile(path).LoadAllAssets().First(x => x is RuntimeAnimatorController);
            }

            animatorController = animatorObject as RuntimeAnimatorController;
            return true;
        }

        return false;
    }

    /*
    private static void CreateTeleporters()
    {
        string teleportPath = Path.Combine(_schematicDirectoryPath, $"{_schematicName}-Teleports.json");
        if (!File.Exists(teleportPath))
            return;

        foreach (SerializableTeleport teleport in JsonConvert.DeserializeObject<List<SerializableTeleport>>(File.ReadAllText(teleportPath)))
        {
            GameObject gameObject = Object.Instantiate(_blockPrefabs.FirstOrDefault(x => x.name == "Teleporter"));
            gameObject.name = teleport.Name;
            gameObject.transform.parent = _objectFromId[teleport.ParentId];
            gameObject.transform.localPosition = teleport.Position;
            gameObject.transform.localEulerAngles = teleport.Rotation;
            gameObject.transform.localScale = teleport.Scale;

            if (gameObject.TryGetComponent(out TeleportComponent teleportComponent))
            {
                teleportComponent.TargetTeleporters = teleport.TargetTeleporters.ToArray();
                teleportComponent.RoomType = teleport.RoomType;
                teleportComponent.AllowedRoleTypes = teleport.AllowedRoles.ToArray();
                teleportComponent.Cooldown = teleport.Cooldown;
                teleportComponent.TeleportFlags = teleport.TeleportFlags;
                teleportComponent.LockOnEvent = teleport.LockOnEvent;
                teleportComponent.SoundOnTeleport = teleport.TeleportSoundId;

                if (teleport.PlayerRotationX.HasValue)
                {
                    teleportComponent.OverridePlayerXRotation = true;
                    teleportComponent.PlayerRotationX = teleport.PlayerRotationX.Value;
                }

                if (teleport.PlayerRotationY.HasValue)
                {
                    teleportComponent.OverridePlayerYRotation = true;
                    teleportComponent.PlayerRotationY = teleport.PlayerRotationY.Value;
                }
            }

            _objectFromId.Add(teleport.ObjectId, gameObject.transform);
        }

        foreach (TeleportComponent teleport in _rootTransform.GetComponentsInChildren<TeleportComponent>())
        {
            foreach (TargetTeleporter targetTeleporter in teleport.TargetTeleporters)
            {
                targetTeleporter.Teleporter = _objectFromId[targetTeleporter.Id].GetComponent<TeleportComponent>();
            }
        }
    }
    */

    private static void AddRigidbodies()
    {
        string rigidbodyPath = Path.Combine(_schematicDirectoryPath, $"{_schematicName}-Rigidbodies.json");
        if (!File.Exists(rigidbodyPath))
            return;

        foreach (KeyValuePair<int, SerializableRigidbody> dict in JsonConvert.DeserializeObject<Dictionary<int, SerializableRigidbody>>(File.ReadAllText(rigidbodyPath)))
        {
            if (!_objectFromId[dict.Key].gameObject.TryGetComponent(out Rigidbody rigidbody))
                rigidbody = _objectFromId[dict.Key].gameObject.AddComponent<Rigidbody>();

            rigidbody.isKinematic = dict.Value.IsKinematic;
            rigidbody.useGravity = dict.Value.UseGravity;
            rigidbody.constraints = dict.Value.Constraints;
            rigidbody.mass = dict.Value.Mass;
        }
    }

    private static void NullifyFields()
    {
        _rootTransform = null;
        _schematicName = null;
        _schematicDirectoryPath = null;
        _schematicData = null;
        _objectFromId = null;
        AssetBundle.UnloadAllAssetBundles(false);
    }

    private static Transform _rootTransform;
    private static string _schematicName;
    private static string _schematicDirectoryPath;
    private static SchematicObjectDataList _schematicData;
    private static Dictionary<int, Transform> _objectFromId;
}

