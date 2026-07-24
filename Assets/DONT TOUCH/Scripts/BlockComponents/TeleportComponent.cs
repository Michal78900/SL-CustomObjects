using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class TeleportComponent : SchematicBlock
{
    public override BlockType BlockType => BlockType.Teleport;

    public string Id;

    // public RoomType RoomType = RoomType.Surface;

    /*
    [ReorderableList]
    [ValidateInput("ValidateList", "The target teleport list is invalid. Make sure that your list:\n" +
                                   "- Does not contain any duplicates\n" +
                                   "- One of the teleporters does not point to itself")]
                                   */
    public TargetTeleport[] TargetTeleporters; // = new[] { new TargetTeleporter() /*{ ChanceToTeleport = 100 }*/ };

    // [BoxGroup("Teleport properties")] [ReorderableList]
    /*
    public string[] AllowedRoleTypes =
    {
        "Scp173",
        "ClassD",
        "Spectator",
        "Scp106",
        "NtfSpecialist",
        "Scp049",
        "Scientist",
        "Scp079",
        "ChaosConscript",
        "Scp096",
        "Scp0492",
        "NtfSergeant",
        "NtfCaptain",
        "NtfPrivate",
        "Tutorial",
        "FacilityGuard",
        "Scp939",
        "CustomRole",
        "ChaosRifleman",
        "ChaosMarauder",
        "ChaosRepressor",
        "Overwatch",
        "Filmmaker",
        "Scp3114"
    };
    */

    // [BoxGroup("Teleport properties")]
    public float Cooldown = 10f;

    // [BoxGroup("Teleport properties")]
    // public TeleportFlags TeleportFlags = TeleportFlags.Player;

    // [BoxGroup("Teleport properties")]
    // public LockOnEvent LockOnEvent = LockOnEvent.None;

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
    // public bool PlaySoundOnTeleport = false;

    // [BoxGroup("Player properties")]
    // [ShowIf("PlaySoundOnTeleport")]
    // [Range(0, 31)]
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
    // public int SoundOnTeleport;

    // [BoxGroup("Player properties")]
    // [ShowIf("TeleportFlags", TeleportFlags.Player)]
    // public bool OverridePlayerXRotation = false;

    // [BoxGroup("Player properties")] [ShowIf("OverridePlayerXRotation")]
    // [Range(-360f, 360f)]
    // public float PlayerRotationX;

    // [BoxGroup("Player properties")] [ShowIf("TeleportFlags", TeleportFlags.Player)]
    // public bool OverridePlayerYRotation = false;

    // [BoxGroup("Player properties")] [ShowIf("OverridePlayerYRotation")]
    // [Range(-360f, 360f)]
    // public float PlayerRotationY;

    public override void Compile(SchematicBlockData block)
    {
        /*
        if (!ValidateList(TargetTeleporters))
            throw new Exception($"The teleport list for the {name} is invalid! ({name})");

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
        */
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

        if (string.IsNullOrEmpty(Id))
            Id = Guid.NewGuid().ToString("N").Substring(0, 8);

        foreach (TargetTeleport teleport in TargetTeleporters)
        {
            if (teleport.Teleport != null)
                teleport.Id = teleport.Teleport.Id;    
        }
    }

    private bool ValidateList(TargetTeleport[] array)
    {
        List<TeleportComponent> checkList = new List<TeleportComponent>();

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Teleport == null || array[i].Teleport == this || checkList.Contains(array[i].Teleport))
                return false;

            checkList.Add(array[i].Teleport);
        }

        return true;
    }
}

[Serializable]
public class TargetTeleport
{
    public string Id;

    // public float Chance { get; set; }

    [JsonIgnore] [Tooltip("Drag and drop target teleporter here.")]
    public TeleportComponent Teleport;

    // [JsonIgnore] [Tooltip("Set chance of teleporting to this teleporter.")]
    // public float ChanceToTeleport = 100f;
}