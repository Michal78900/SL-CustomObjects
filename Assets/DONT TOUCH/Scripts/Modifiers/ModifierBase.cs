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
            
            Apply = false;
            GameObject copy = Instantiate(transform.root.gameObject);
            copy.SetActive(false);

            ApplyModifier();
        }

        protected abstract void PreviewModifier();

        internal virtual void ApplyModifier() => DestroyImmediate(this);
    }
}