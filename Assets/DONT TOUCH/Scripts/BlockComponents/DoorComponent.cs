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

	public override bool Compile(SchematicBlockData block, Schematic _)
	{
		block.BlockType = BlockType;
		
		block.Properties = new Dictionary<string, object>
		{
			{ "DoorType", DoorType },
			{ "IsOpen", IsOpen },
			{ "IsLocked", IsLocked },
			{ "RequiredPermissions", RequiredPermissions },
			{ "RequireAll", RequireAll },
		};
		
		return true;
	}
}
