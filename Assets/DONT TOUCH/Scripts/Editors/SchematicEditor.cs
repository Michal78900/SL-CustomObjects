namespace DONT_TOUCH.Scripts.Editors
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(Schematic))]
    public class SchematicEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Schematic schematic = (Schematic)target;

            GUILayout.Label($"<color=white>Number of blocks: <b>{schematic.GetComponentsInChildren<SchematicBlock>().Length - 1}</b></color>", SchematicManager.UnityRichTextStyle);

            if (GUILayout.Button("Apply Rotation to Empty Objects"))
            {
                int i = ApplyTransformProperty(schematic, true, false);
                Debug.Log(
                    i > 0
                        ? $"<color=#00FF00>Successfully applied rotation to <b>{i}</b> empties!</color>"
                        : "<color=#FFFF00>No empties have been found to which rotation could be applied to.</color>");

                return;
            }

            if (GUILayout.Button("Apply Scale to Empty Objects"))
            {
                int i = ApplyTransformProperty(schematic, false, true);
                Debug.Log(
                    i > 0
                        ? $"<color=#00FF00>Successfully applied scale to <b>{i}</b> empties!</color>"
                        : "<color=#FFFF00>No empties have been found to which scale could be applied to.</color>");

                return;
            }

            if (GUILayout.Button("Compile"))
                schematic.CompileSchematic();
        }

        private static int ApplyTransformProperty(Schematic schematic, bool rotation, bool scale)
        {
            int i = 0;
            foreach (Transform empty in schematic.GetComponentsInChildren<Transform>())
            {
                // Empty is an object that has only one transform component.
                if (empty.GetComponents<Component>().Length > 1)
                    continue;

                // Make sure it isn't a SchematicBlock, just in case.
                if (empty.TryGetComponent(out SchematicBlock _))
                    continue;

                // Skip if empty has rotation or scale already applied.
                if (empty.transform.localScale == Vector3.one && empty.transform.localEulerAngles == Vector3.zero)
                    continue;

                i++;

                // Get child primitives from empty.
                PrimitiveComponent[] primitives = empty.GetComponentsInChildren<PrimitiveComponent>();
                Dictionary<Transform, Transform> transformToParent = new();

                // Save primitive's transform parents and temporally remove them.
                foreach (PrimitiveComponent primitiveComponent in primitives)
                {
                    Transform primitiveTransform = primitiveComponent.transform;
                    transformToParent.Add(primitiveTransform, primitiveTransform.parent);
                    primitiveComponent.transform.parent = null;
                }

                if (rotation)
                    empty.transform.localEulerAngles = Vector3.zero;

                if (scale)
                    empty.transform.localScale = Vector3.one;

                // Re-apply saved parent transforms.
                foreach (PrimitiveComponent primitiveComponent in primitives)
                {
                    Transform primitiveTransform = primitiveComponent.transform;
                    primitiveTransform.parent = transformToParent[primitiveTransform];
                }
            }

            return i;
        }
    }
}