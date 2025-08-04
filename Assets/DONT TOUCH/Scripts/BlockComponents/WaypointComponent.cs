using System;
using UnityEngine;

[ExecuteInEditMode]
public class WaypointComponent : SchematicBlock
{
	public override BlockType BlockType => BlockType.Waypoint;

	public override void Compile(SchematicBlockData block) => base.Compile(block);

	public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
	{
		gameObject = Create<GameObject>("Assets/Resources/Blocks/Waypoint.prefab");
		base.Decompile(ref gameObject, block, parent);
	}

	private void Start()
	{
		TryGetComponent(out _filter);
		TryGetComponent(out _renderer);
	}

	private void Update()
	{
		_filter.hideFlags = HideFlags.HideInInspector;
		_renderer.hideFlags = HideFlags.HideInInspector;
	}

	private MeshFilter _filter;
	private MeshRenderer _renderer;

}
