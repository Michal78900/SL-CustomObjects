using System;
using UnityEngine;

public class EmptyComponent : SchematicBlock
{
	public override BlockType BlockType => BlockType.Empty;

	public override void Compile(SchematicBlockData block) => base.Compile(block);

	public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
	{
		gameObject = Create<GameObject>("Assets/Resources/Blocks/Empty.prefab");
		base.Decompile(ref gameObject, block, parent);
	}
}
