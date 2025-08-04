using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, SelectionBase]
public class DoorComponent : SchematicBlock
{
	public override BlockType BlockType => BlockType.Door;
	public DoorType DoorType;
	public bool IsOpen;
	public bool IsLocked;
	public DoorPermissionFlags RequiredPermissions = DoorPermissionFlags.None;
	public bool RequireAll = true;

	public override void Compile(SchematicBlockData block)
	{
		block.Properties = new Dictionary<string, object>
		{
			{ "DoorType", DoorType },
			{ "IsOpen", IsOpen },
			{ "IsLocked", IsLocked },
			{ "RequiredPermissions", RequiredPermissions },
			{ "RequireAll", RequireAll },
		};
		
		base.Compile(block);
	}

	public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
	{
		DoorType doorType = (DoorType)Convert.ToInt32(block.Properties["DoorType"]);
		DoorComponent doorComponent = Create<DoorComponent>($"Assets/Resources/Blocks/Doors/{doorType}.prefab");
		gameObject = doorComponent.gameObject;
		
		doorComponent.RequireAll = Convert.ToBoolean(block.Properties["RequireAll"]);
		doorComponent.IsOpen = Convert.ToBoolean(block.Properties["IsOpen"]);
		doorComponent.IsLocked = Convert.ToBoolean(block.Properties["IsLocked"]);
		doorComponent.RequiredPermissions = (DoorPermissionFlags)Convert.ToUInt16(block.Properties["RequiredPermissions"]);
		base.Decompile(ref gameObject, block, parent);
	}
}
