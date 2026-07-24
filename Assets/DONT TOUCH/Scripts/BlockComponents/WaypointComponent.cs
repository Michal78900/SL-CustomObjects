using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaypointComponent : SchematicBlock
{
	public override BlockType BlockType => BlockType.Waypoint;

	[Tooltip("AI navigation priority. 255 = highest priority (default). Lower = less preferred.")]
	[Range(0, 255)]
	public int Priority = 255;

	public override void Compile(SchematicBlockData block)
	{
		base.Compile(block);
		block.Properties = new Dictionary<string, object>
		{
			{ "Priority", Priority },
		};
	}

	public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
	{
		gameObject = Create<GameObject>("Assets/Resources/Blocks/Waypoint.prefab");
		base.Decompile(ref gameObject, block, parent);

		if (block.Properties != null && block.Properties.TryGetValue("Priority", out object priority))
			Priority = Convert.ToInt32(priority);
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

