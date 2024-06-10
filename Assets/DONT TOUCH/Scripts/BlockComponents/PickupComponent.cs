using System.Collections.Generic;
using UnityEngine;

public class PickupComponent : SchematicBlock
{
    [Tooltip("The ItemType of this pickup.")]
    public ItemType ItemType;
    
    [Header("Custom Item name/ID")]
    public string CustomItem;
    
    public AttachmentName[] Attachments;

    [Header("Chance %")]
    [Tooltip("The chance for this pickup to spawn.")]
    [Range(0f, 100f)]
    public float Chance = 100f;

    [Tooltip("Number of times you can use the pickup it dissappears. Set to -1 for no limit.")]
    [Min(-1)]
    public int NumberOfUses = 1;
    
    [Header("Can be picked up (untick for button)")]
    [Tooltip("Whether the pickup can be picked up in game. If set to false this can be used as custom button via a serperate plugin.")]
    public bool CanBePickedUp = true;

    public override BlockType BlockType => BlockType.Pickup;

    public override bool Compile(SchematicBlockData block, Schematic _)
    {
        block.Rotation = transform.localEulerAngles;
        block.Scale = transform.localScale;

        block.BlockType = BlockType.Pickup;
        block.Properties = new Dictionary<string, object>
        {
            { "ItemType", ItemType },
            { "CustomItem", CustomItem },
            { "Attachments", Attachments },
            { "Chance", Chance },
            { "Uses", NumberOfUses },
        };

        if (!CanBePickedUp)
            block.Properties.Add("Locked", string.Empty);

        return true;
    }

    private void OnValidate()
    {
        if (!CanBePickedUp)
            NumberOfUses = -1;
    }
}

