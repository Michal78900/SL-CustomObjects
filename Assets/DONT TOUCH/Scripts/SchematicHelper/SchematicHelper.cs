using UnityEditor;

[InitializeOnLoad]
public class SchematicHelper : EditorWindow 
{
    static SchematicHelper()
    {

    }
    
    #region Selection
    [MenuItem("MapEditorReborn Tools/Selection/Set Static")]
    private static void SetStaticSelected()
    {
        foreach (var go in Selection.gameObjects)
        {
            go.isStatic = true;
        }
    }
    
    [MenuItem("MapEditorReborn Tools/Selection/Set Dynamic")]
    private static void SetDynamicSelected()
    {
        foreach (var go in Selection.gameObjects)
        {
            go.isStatic = false;
        }
    }
    
    [MenuItem("MapEditorReborn Tools/Selection/Set Collidable")]
    private static void SetCollidableSelected()
    {
        foreach (var go in Selection.gameObjects)
        {
            if (go.TryGetComponent(out PrimitiveComponent collider))
            {
                collider.Collidable = true;
            }
        }
    }
    
    [MenuItem("MapEditorReborn Tools/Selection/Set Non-Collidable")]
    private static void SetNonCollidableSelected()
    {
        foreach (var go in Selection.gameObjects)
        {
            if (go.TryGetComponent(out PrimitiveComponent collider))
            {
                collider.Collidable = false;
            }
        }
    }
    
    [MenuItem("MapEditorReborn Tools/Selection/Set Visible")]
    private static void SetVisibleSelected()
    {
        foreach (var go in Selection.gameObjects)
        {
            if (go.TryGetComponent(out PrimitiveComponent collider))
            {
                collider.Visible = true;
            }
        }
    }
    
    [MenuItem("MapEditorReborn Tools/Selection/Set Invisible")]
    private static void SetInvisibleSelected()
    {
        foreach (var go in Selection.gameObjects)
        {
            if (go.TryGetComponent(out PrimitiveComponent collider))
            {
                collider.Visible = false;
            }
        }
    }
    #endregion
}