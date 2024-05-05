using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class PrimitiveComponent : SchematicBlock
{
    [Tooltip("The color of the primitive. Supports transparent colors.")]
    public Color Color;

    [Tooltip("Whether the primitive should have a collider attached to it.")]
    public bool Collidable;

    [Tooltip("Whether the primitive should be static. Enable it if your schematic contain animation and you want to define this primitive only as a static object.")]
    public bool Static;

    public override BlockType BlockType => BlockType.Primitive;

    public override bool Compile(SchematicBlockData block, Schematic _)
    {
        block.Rotation = transform.eulerAngles;
        Vector3 scaleAbs = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
        block.Scale = Collidable ? scaleAbs : scaleAbs * -1f;

        block.BlockType = BlockType.Primitive;
        block.Properties = new Dictionary<string, object>
        {
            { "PrimitiveType", (PrimitiveType)Enum.Parse(typeof(PrimitiveType), tag) },
            { "Color", ColorUtility.ToHtmlStringRGBA(Color) },
			{ "IsStatic", Static }
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

    internal MeshFilter _filter;
    private MeshRenderer _renderer;
    private Material _sharedRegular;
    private Material _sharedTransparent;
}