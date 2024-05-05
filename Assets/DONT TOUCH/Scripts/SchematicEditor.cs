using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#pragma warning disable CS0618

[CustomEditor(typeof(Schematic))]
public class SchematicEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Schematic schematic = (Schematic)target;

        DrawDefaultInspector();
        
        if (GUILayout.Button("Generate"))
        {
            schematic.CompileSchematic();
        }
        
        if (GUILayout.Button("Open folder"))
        {
            var schematicFolder = SchematicManager.Config.ExportPath + "/" + schematic.gameObject.name;
            
            if (!Directory.Exists(schematicFolder))
                Directory.CreateDirectory(schematic.CompileSchematic());
            
            System.Diagnostics.Process.Start(schematicFolder);
        }
    }
}