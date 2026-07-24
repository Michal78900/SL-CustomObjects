using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class InteractableComponent : SchematicBlock
{
	public ColliderShape Shape;

	[Min(0f)]
	public float InteractionDuration;

	public bool IsLocked;

	[Header("Animation Trigger Settings")]
	[Tooltip("Drag and drop the target GameObject (containing the Animator component) here.")]
	public GameObject TargetObject;
	[Tooltip("The animation state to play on interaction.")]
	public string AnimationStateName;
	[Tooltip("An optional second state to toggle to on subsequent interactions.")]
	public string AnimationStateName2;

	public override BlockType BlockType => BlockType.Interactable;

	public override void Compile(SchematicBlockData block)
	{
		int targetAnimatorId = 0;
		if (TargetObject != null)
		{
			targetAnimatorId = TargetObject.transform.GetInstanceID();
		}

		block.Properties = new Dictionary<string, object>
		{
			{ "Shape", Shape },
			{ "InteractionDuration", InteractionDuration },
			{ "IsLocked", IsLocked },
			{ "TargetAnimatorId", targetAnimatorId },
			{ "AnimationStateName", AnimationStateName },
			{ "AnimationStateName2", AnimationStateName2 }
		};

		base.Compile(block);
	}

	public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
	{
		InteractableComponent interactable = Instantiate(AssetDatabase.LoadAssetAtPath<InteractableComponent>("Assets/Resources/Blocks/Interactable.prefab"));
        gameObject = interactable.gameObject;

		interactable.Shape = (ColliderShape)Convert.ToInt32(block.Properties["Shape"]);
		interactable.InteractionDuration = Convert.ToSingle(block.Properties["InteractionDuration"]);
		interactable.IsLocked = block.Properties.TryGetValue("IsLocked", out object isLocked) && Convert.ToBoolean(isLocked);
		
		interactable.AnimationStateName = block.Properties.TryGetValue("AnimationStateName", out object state) ? Convert.ToString(state) : string.Empty;
		interactable.AnimationStateName2 = block.Properties.TryGetValue("AnimationStateName2", out object state2) ? Convert.ToString(state2) : string.Empty;

		base.Decompile(ref gameObject, block, parent);
	}

	private void Update()
	{
		if (_collider != null)
		{
			_collider.isTrigger = false;

			switch (_collider)
			{
				case BoxCollider boxCollider:
					boxCollider.center = Vector3.zero;
					boxCollider.size = Vector3.one;
					break;

				case SphereCollider sphereCollider:
					sphereCollider.center = Vector3.zero;
					sphereCollider.radius = 0.5f;
					break;

				case CapsuleCollider capsuleCollider:
					capsuleCollider.center = Vector3.zero;
					capsuleCollider.radius = 0.5f;
					capsuleCollider.height = 2.0f;
					capsuleCollider.direction = 1;
					break;
			}
		}

		if (_prevShape == Shape)
			return;

		DestroyImmediate(_collider);

		switch (Shape)
		{
			case ColliderShape.Box:
				_collider = gameObject.AddComponent<BoxCollider>();
				break;
			case ColliderShape.Sphere:
				_collider = gameObject.AddComponent<SphereCollider>();
				break;
			case ColliderShape.Capsule:
				CapsuleCollider capsule = gameObject.AddComponent<CapsuleCollider>();
				capsule.height = 2.0f;
				capsule.radius = 0.5f;
				_collider = capsule;
				break;
		}

		_prevShape = Shape;
		_collider.hideFlags = HideFlags.HideInInspector;
	}

	private Collider _collider;
	private ColliderShape? _prevShape;
}
