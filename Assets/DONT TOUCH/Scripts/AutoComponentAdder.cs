namespace DONT_TOUCH.Scripts
{
    using System.Collections.Generic;
    using Unity.VisualScripting;
    using UnityEditor;
    using UnityEngine;
    
    public class AutoComponentAdder : MonoBehaviour
    {
        private static readonly Dictionary<Mesh, GameObject> MeshToPrefab = new();

        static AutoComponentAdder()
        {
            ObjectChangeEvents.changesPublished += OnChangesPublished;
        }

        private static void OnChangesPublished(ref ObjectChangeEventStream stream)
        {
            if (!Config.AutoAddComponents)
                return;

            if (MeshToPrefab.Count == 0)
            {
                foreach (GameObject prefab in Resources.LoadAll<GameObject>("Blocks/Primitives"))
                {
                    if (!prefab.TryGetComponent(out MeshFilter meshFilter))
                        continue;

                    MeshToPrefab.Add(meshFilter.sharedMesh, prefab);
                }
            }

            for (int i = 0; i < stream.length; i++)
            {
                ObjectChangeKind changeKind = stream.GetEventType(i);
                if (changeKind != ObjectChangeKind.CreateGameObjectHierarchy)
                    continue;

                stream.GetCreateGameObjectHierarchyEvent(i, out CreateGameObjectHierarchyEventArgs ev);
                GameObject instance = EditorUtility.InstanceIDToObject(ev.instanceId).GameObject();

                if (instance.TryGetComponent(out MeshFilter meshFilter))
                {
                    // Ignore already existing SchematicBlocks
                    if (instance.TryGetComponent(out SchematicBlock _))
                        continue;
                    
                    ReplaceWithPrimitive(instance, meshFilter);
                }
                else
                {
                    // Only apply for empties at the top of hierarchy. Empties also have only one component present.
                    if (instance.transform.parent != null || instance.GetComponents<Component>().Length != 1)
                        continue;

                    instance.AddComponent<Schematic>();
                    instance.name = "NewSchematic";
                }
            }
        }

        private static void ReplaceWithPrimitive(GameObject var, MeshFilter meshFilter)
        {
            GameObject replacement = Instantiate(MeshToPrefab[meshFilter.sharedMesh], var.transform.parent, true);
            replacement.transform.SetLocalPositionAndRotation(var.transform.localPosition, var.transform.localRotation);
            replacement.name = var.name;
            Selection.activeTransform = replacement.transform;
            DestroyImmediate(var);
        }

        private static readonly Config Config = SchematicManager.Config;
    }
}