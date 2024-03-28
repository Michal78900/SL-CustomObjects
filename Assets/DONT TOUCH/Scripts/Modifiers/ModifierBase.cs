namespace DONT_TOUCH.Scripts.Modifiers
{
    using UnityEngine;

    [ExecuteInEditMode]
    public abstract class ModifierBase : MonoBehaviour
    {
        public bool Apply;

        private void OnDrawGizmos()
        {
            if (!enabled)
                return;
            
            PreviewModifier();
        }

        private void Update()
        {
            if (!Apply)
                return;
            
            ApplyModifier();
        }

        protected abstract void PreviewModifier();

        virtual internal void ApplyModifier()
        {
            Apply = false;
            DestroyImmediate(this);
        }
    }
}