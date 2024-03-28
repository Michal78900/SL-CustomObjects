namespace DONT_TOUCH.Scripts.Modifiers
{
    using UnityEngine;

    public class MirrorModifier : ModifierBase
    {
        public bool AxisX = true, AxisY, AxisZ;
        
        protected override void PreviewModifier()
        {
            foreach (PrimitiveComponent primitive in GetComponentsInChildren<PrimitiveComponent>())
            {
                Gizmos.color = primitive.Color;

                Vector3 mirrorPosition = primitive.transform.position;
                if (AxisX)
                    mirrorPosition.x *= -1f;

                if (AxisY)
                    mirrorPosition.y *= -1f;

                if (AxisZ)
                    mirrorPosition.z *= -1f;
                
                Gizmos.DrawMesh(primitive._filter.sharedMesh, -1, mirrorPosition, primitive.transform.rotation, primitive.transform.localScale);
            }
        }

        override internal void ApplyModifier()
        {
            GameObject copy = Instantiate(gameObject, transform.parent);
            DestroyImmediate(copy.GetComponent<MirrorModifier>());
            foreach (Transform copyTransform in copy.GetComponentsInChildren<Transform>())
            {
                Vector3 mirrorPosition = copyTransform.transform.localPosition;
                if (AxisX)
                    mirrorPosition.x *= -1f;

                if (AxisY)
                    mirrorPosition.y *= -1f;

                if (AxisZ)
                    mirrorPosition.z *= -1f;

                copyTransform.transform.localPosition = mirrorPosition;
            }
            
            base.ApplyModifier();
        }
    }
}