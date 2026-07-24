using System;
using System.Collections.Generic;
using UnityEngine;

public class PickupComponent : SchematicBlock
{
    [Tooltip("The ItemType of this pickup.")]
    public ItemType ItemType;
    
    [Tooltip("Custom Item name/ID.")]
    public string CustomItem;

    [Tooltip("Use the in-game RA command \"forceatt\" while holding a firearm to get the code.\nSet to -1 for random attachments.")]
    public string AttachmentsCode;

    [Tooltip("The chance (in %) for this pickup to spawn.")]
    [Range(0f, 100f)]
    public float Chance = 100f;

    [Tooltip("Number of times you can use the pickup before it dissappears.\nSet to -1 for no limit.")]
    [Min(-1)]
    public int NumberOfUses = 1;

    [Tooltip("If true, this pickup cannot be picked up and instead acts as an interactable button.")]
    public bool Locked = false;

    public override BlockType BlockType => BlockType.Pickup;

    public override void Compile(SchematicBlockData block)
    {
        block.Properties = new Dictionary<string, object>
        {
            { "ItemType", ItemType },
            { "CustomItem", CustomItem },
            { "AttachmentsCode", AttachmentsCode },
            { "Chance", Chance },
            { "Uses", NumberOfUses },
        };
        if (Locked)
        {
            block.Properties.Add("Locked", true);
        }

        base.Compile(block);
    }

	public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
	{
        PickupComponent pickupComponent = Create<PickupComponent>("Assets/Resources/Blocks/Pickup.prefab");
        gameObject = pickupComponent.gameObject;

        pickupComponent.ItemType = (ItemType)Convert.ToInt32(block.Properties["ItemType"]);
        pickupComponent.CustomItem = block.Properties["CustomItem"].ToString();
        pickupComponent.AttachmentsCode = block.Properties.TryGetValue("AttachmentsCode", out object attachmentsCode) ? attachmentsCode.ToString() : "-1";
        pickupComponent.Chance = Convert.ToSingle(block.Properties["Chance"]);
        pickupComponent.NumberOfUses = Convert.ToInt32(block.Properties["Uses"]);
        pickupComponent.Locked = block.Properties.ContainsKey("Locked");

		base.Decompile(ref gameObject, block, parent);
	}

    private void OnValidate()
    {
        if (!uint.TryParse(AttachmentsCode, out uint _))
            AttachmentsCode = "-1";
    }
}

