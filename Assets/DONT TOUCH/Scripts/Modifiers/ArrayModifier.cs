namespace DONT_TOUCH.Scripts.Modifiers
{
    using UnityEngine;

    public class ArrayModifier : ModifierBase
    {
        [Min(1)]
        public uint Count = 2;

        public Vector3 RelativeOffset = new(1f, 0f, 0f);

        protected override void PreviewModifier()
        {
            foreach (PrimitiveComponent primitive in GetComponentsInChildren<PrimitiveComponent>())
            {
                Gizmos.color = primitive.Color;
                
                for (int i = 1; i < Count; i++)
                    Gizmos.DrawMesh(primitive._filter.sharedMesh, -1, primitive.transform.position + RelativeOffset * i, primitive.transform.rotation, primitive.transform.localScale);
            }
        }

        override internal void ApplyModifier()
        {
            for (int i = 1; i < Count; i++)
            {
                GameObject copy = Instantiate(gameObject, transform.parent);
                DestroyImmediate(copy.GetComponent<ArrayModifier>());
                copy.transform.SetLocalPositionAndRotation(transform.localPosition + RelativeOffset * i, transform.rotation);
            }
            
            base.ApplyModifier();
        }
    }
}