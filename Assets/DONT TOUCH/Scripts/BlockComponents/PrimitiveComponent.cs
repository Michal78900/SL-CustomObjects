using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts.Editors;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class PrimitiveComponent : SchematicBlock
{
    [Tooltip("The color of the primitive. Supports transparent colors.")]
    public Color Color;

    [Tooltip("Whether the primitive should have a collider attached to it.")]
    public bool Collidable = true;

    [Tooltip("Whether the primitive should be visible in game.")]
    public bool Visible = true;

    [Tooltip("Snaps the object rotation to real in-game rotation.")]
    public bool SnapRotation;

    public override BlockType BlockType => BlockType.Primitive;

    public override bool Compile(SchematicBlockData block, Schematic _)
    {
        block.Rotation = transform.eulerAngles;
        block.Scale = transform.localScale;
        block.BlockType = BlockType.Primitive;

        PrimitiveFlags primitiveFlags = PrimitiveFlags.None;
        if (Collidable)
            primitiveFlags |= PrimitiveFlags.Collidable;

        if (Visible)
            primitiveFlags |= PrimitiveFlags.Visible;

        block.Properties = new Dictionary<string, object>
        {
            { "PrimitiveType", (PrimitiveType)Enum.Parse(typeof(PrimitiveType), tag) },
            { "Color", ColorUtility.ToHtmlStringRGBA(Color) },
            { "PrimitiveFlags", primitiveFlags },
            { "Static", gameObject.isStatic }
        };

        return true;
    }

    private void Start()
    {
        TryGetComponent(out _filter);
        TryGetComponent(out _renderer);
        _sharedRegular = new Material((Material)Resources.Load("Materials/Regular"));
        _sharedTransparent = new Material((Material)Resources.Load("Materials/Transparent"));
    }

    private void Update()
    {
        _filter.hideFlags = HideFlags.HideInInspector;
        _renderer.hideFlags = HideFlags.HideInInspector;

#if UNITY_EDITOR
        if (EditorUtility.IsPersistent(gameObject))
            return;
#endif

        _renderer.sharedMaterial = Color.a >= 1f ? _sharedRegular : _sharedTransparent;
        _renderer.sharedMaterial.color = Color;
        
        _renderer.enabled = Visible;
        
        if (SnapRotation)
            transform.localRotation = PrimitiveEditor.GetRealRotation(this);
    }

    private void OnDrawGizmos()
    {
        if (Visible)
            return;
        
        Gizmos.color = new Color(Color.r, Color.g, Color.b, 1f);
        Gizmos.DrawWireMesh(_filter.sharedMesh, 0, transform.position, transform.rotation, transform.localScale);
    }

    internal MeshFilter _filter;
    private MeshRenderer _renderer;
    private Material _sharedRegular;
    private Material _sharedTransparent;
}