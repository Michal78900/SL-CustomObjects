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
    public bool Collidable;

    [Tooltip("Whether the primitive should be visible in game.")]
    public bool Visible;

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
            { "PrimitiveFlags", primitiveFlags }
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
    }

    private void OnDrawGizmos()
    {
        Quaternion realRotation = PrimitiveEditor.GetRealRotation(this);
        
        if (Visible)
        {
            _renderer.enabled = true;
            // if (realRotation.eulerAngles != transform.localEulerAngles)
            // {
                // Gizmos.color = new Color(1f - Color.r, 1f - Color.g, 1f - Color.b, 1f);
                // Gizmos.DrawWireMesh(_filter.sharedMesh, 0, transform.position, Quaternion.Euler(realRotation.eulerAngles), transform.localScale);
            // }
            
            return;
        }
        
        _renderer.enabled = false;
        Gizmos.color = new Color(Color.r, Color.g, Color.b, 1f);
        Gizmos.DrawWireMesh(_filter.sharedMesh, 0, transform.position, Quaternion.Euler(realRotation.eulerAngles), transform.localScale);
    }

    internal MeshFilter _filter;
    private MeshRenderer _renderer;
    private Material _sharedRegular;
    private Material _sharedTransparent;
}