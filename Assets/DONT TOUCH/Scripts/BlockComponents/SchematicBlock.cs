using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class SchematicBlock : MonoBehaviour
{
    public abstract BlockType BlockType { get; }

    public static T Create<T>(string prefabPath) where T : UnityEngine.Object
    {
        T prefab = AssetDatabase.LoadAssetAtPath<T>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"{prefabPath} not found.");
            return null;
        }

        T instance = Instantiate(prefab);
        instance.name = instance.name.Replace("(Clone)", string.Empty);

        return instance;
    }

    public virtual void Compile(SchematicBlockData block)
    {
        block.BlockType = BlockType;

        Transform t = transform;
        block.Name = t.name;
        block.ObjectId = t.GetInstanceID();
        block.ParentId = t.parent.GetInstanceID();

        t.GetLocalPositionAndRotation(out Vector3 localPosition, out Quaternion localRotation);
        block.Position = localPosition;
        block.Rotation = localRotation.eulerAngles;
        block.Scale = t.localScale;

        if (block.Properties != null)
        {
            block.Properties.Add("Static", gameObject.isStatic);
        }
        else
        {
            block.Properties = new Dictionary<string, object>()
            {
                { "Static", gameObject.isStatic }
            };
        }
    }

    public virtual void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
    {
        Transform t = gameObject.transform;
        t.name = block.Name;

        t.SetParent(parent);
        t.localPosition = block.Position;
        t.localEulerAngles = block.Rotation != null ? block.Rotation : Vector3.zero;
        t.localScale = block.Scale != null ? block.Scale == Vector3.zero ? Vector3.one : block.Scale : Vector3.one;

        gameObject.isStatic = block.Properties != null && block.Properties.TryGetValue("Static", out object isStatic) && Convert.ToBoolean(isStatic);
    }
}
