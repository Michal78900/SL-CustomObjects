using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class TeleportComponent : SchematicBlock
{
    public override BlockType BlockType => BlockType.Teleport;

    public RoomType RoomType = RoomType.Surface;

    /*
    [ReorderableList]
    [ValidateInput("ValidateList", "The target teleport list is invalid. Make sure that your list:\n" +
                                   "- Does not contain any duplicates\n" +
                                   "- One of the teleporters does not point to itself")]
                                   */
    public TargetTeleporter[] TargetTeleporters = new[] { new TargetTeleporter { ChanceToTeleport = 100 } };

    // [BoxGroup("Teleport properties")] [ReorderableList]
    public string[] AllowedRoleTypes = 
    {
        "Scp0492",
        "Scp049",
        "Scp096",
        "Scp106",
        "Scp173",
        "Scp939",
        "ClassD",
        "Scientist",
        "FacilityGuard",
        "NtfPrivate",
        "NtfSergeant",
        "NtfSpecialist",
        "NtfCaptain",
        "ChaosConscript",
        "ChaosRifleman",
        "ChaosRepressor",
        "ChaosMarauder",
        "Tutorial",
    };

    // [BoxGroup("Teleport properties")]
    public float Cooldown = 10f;

    // [BoxGroup("Teleport properties")]
    public TeleportFlags TeleportFlags = TeleportFlags.Player;

    // [BoxGroup("Teleport properties")]
    public LockOnEvent LockOnEvent = LockOnEvent.None;

    // [BoxGroup("Player properties")]
    // [ShowIf("TeleportFlags", TeleportFlags.Player)]
    /*
    [Tooltip("Plays the sound to the player on teleport.\n" +
             "Recommended values are:\n" +
             "- 2\n" +
             "- 6\n" +
             "- 7\n" +
             "- 24\n" +
             "- 27\n" +
             "- 30\n" +
             "- 31")]
    */
    public bool PlaySoundOnTeleport = false;

    // [BoxGroup("Player properties")]
    // [ShowIf("PlaySoundOnTeleport")]
    [Range(0, 31)]
    [Tooltip("Plays the sound to the player on teleport.\n" +
             "Recommended values are:\n" +
             "- 2\n" +
             "- 6\n" +
             "- 7\n" +
             "- 24\n" +
             "- 27\n" +
             "- 30\n" +
             "- 31")]
    public int SoundOnTeleport;

    // [BoxGroup("Player properties")]
    // [ShowIf("TeleportFlags", TeleportFlags.Player)]
    public bool OverridePlayerXRotation = false;

    // [BoxGroup("Player properties")] [ShowIf("OverridePlayerXRotation")]
    [Range(-360f, 360f)]
    public float PlayerRotationX;

    // [BoxGroup("Player properties")] [ShowIf("TeleportFlags", TeleportFlags.Player)]
    public bool OverridePlayerYRotation = false;

    // [BoxGroup("Player properties")] [ShowIf("OverridePlayerYRotation")]
    [Range(-360f, 360f)]
    public float PlayerRotationY;

    public override bool Compile(SchematicBlockData block, Schematic schematic)
    {
        if (!ValidateList(TargetTeleporters))
            throw new Exception($"The teleport list for the {name} is invalid! ({name})");
        
        block.Rotation = transform.localEulerAngles;
        block.Scale = transform.localScale;

        SerializableTeleport serializableTeleport = new SerializableTeleport(block)
        {
            RoomType = RoomType,
            TargetTeleporters = new List<TargetTeleporter>(TargetTeleporters.Length),
            AllowedRoles = AllowedRoleTypes.ToList(),
            Cooldown = Cooldown,
            TeleportSoundId = SoundOnTeleport,
            TeleportFlags = TeleportFlags,
            LockOnEvent = LockOnEvent,
        };

        if (!PlaySoundOnTeleport)
            serializableTeleport.TeleportSoundId = -1;

        if (OverridePlayerXRotation &&
            TeleportFlags.HasFlag(TeleportFlags.Player))
            serializableTeleport.PlayerRotationX = PlayerRotationX;

        if (OverridePlayerYRotation &&
            TeleportFlags.HasFlag(TeleportFlags.Player))
            serializableTeleport.PlayerRotationY = PlayerRotationY;

        for (int i = 0; i < TargetTeleporters.Length; i++)
        {
            if (TargetTeleporters[i].Teleporter == null)
                continue;

            TargetTeleporters[i].Id = TargetTeleporters[i].Teleporter.transform.GetInstanceID();
            TargetTeleporters[i].Chance = TargetTeleporters[i].ChanceToTeleport;
        }

        serializableTeleport.TargetTeleporters = TargetTeleporters.ToList();

        schematic.Teleports.Add(serializableTeleport);

        return false;
    }


    private MeshFilter _filter;
    private MeshRenderer _renderer;
    
    private void Awake()
    {
        TryGetComponent(out _filter);
        TryGetComponent(out _renderer);
    }

    private void Update()
    {
        _filter.hideFlags = HideFlags.HideInInspector;
        _renderer.hideFlags = HideFlags.HideInInspector;
    }

    private bool ValidateList(TargetTeleporter[] array)
    {
        List<TeleportComponent> checkList = new List<TeleportComponent>();

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Teleporter == null || array[i].Teleporter == this || checkList.Contains(array[i].Teleporter))
                return false;

            checkList.Add(array[i].Teleporter);
        }

        return true;
    }
}

[Serializable]
public class TargetTeleporter
{
    public int Id { get; set; }

    public float Chance { get; set; }

    [JsonIgnore] [Tooltip("Drag and drop target teleporter here.")]
    public TeleportComponent Teleporter;

    [JsonIgnore] [Tooltip("Set chance of teleporting to this teleporter.")]
    public float ChanceToTeleport = 100f;
}