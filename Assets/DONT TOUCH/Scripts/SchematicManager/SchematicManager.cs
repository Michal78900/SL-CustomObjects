using Newtonsoft.Json;
using System;
using System.IO;
using DONT_TOUCH.Scripts.Modifiers;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SchematicManager : EditorWindow
{
    public static Config Config { get; private set; }

    public static string ConfigPath { get; }

    static SchematicManager()
    {
        EditorApplication.playModeStateChanged += LogPlayModeState;

        ConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "config.json");
        Config = File.Exists(ConfigPath) ? JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigPath)) : new Config();
        _prevConfig = new Config(Config);
    }

    [MenuItem("SchematicManager/Compile all _F6")]
    private static void CompileAllButton()
    {
        Debug.ClearDeveloperConsole();

        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (FindObjectsOfType<ModifierBase>().Length > 0)
            {
                EditorApplication.ExecuteMenuItem("Edit/Play");
                return;
            }
        }

        CompileAll();
    }

    [MenuItem("SchematicManager/Open schematics directory")]
    private static void OpenDirectory()
    {
        if (!Directory.Exists(Config.ExportPath))
            Directory.CreateDirectory(Config.ExportPath);

        System.Diagnostics.Process.Start(Config.ExportPath);
    }

    private static void LogPlayModeState(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.EnteredPlayMode)
            return;

        ModifierBase[] modifiers = FindObjectsOfType<ModifierBase>();
        if (modifiers.Length > 0)
        {
            foreach (ModifierBase modifierBase in modifiers)
            {
                modifierBase.Apply = false;
                modifierBase.ApplyModifier();
                DestroyImmediate(modifierBase);
            }
        }

        CompileAll();
    }

    private static void CompileAll()
    {
        foreach (Schematic schematic in FindObjectsOfType<Schematic>())
        {
            schematic.CompileSchematic();
        }

        if (Config.OpenDirectoryAfterCompiling)
            OpenDirectory();
    }

    [MenuItem("SchematicManager/Settings")]
    private static void ShowWindow() => GetWindow<SchematicManager>("SchematicManager");

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 2000, 2000));
        GUILayout.Label("<size=30><color=white><b>Settings</b></color></size>", UnityRichTextStyle);

        Config.OpenDirectoryAfterCompiling = EditorGUILayout.ToggleLeft(
            "<color=white><i>Open schematics directory after compiling</i></color>",
            Config.OpenDirectoryAfterCompiling,
            UnityRichTextStyle);

        Config.ZipCompiledSchematics = EditorGUILayout.ToggleLeft(
            "<color=white><i>Put compiled schematics directly into .zip archives</i></color>",
            Config.ZipCompiledSchematics,
            UnityRichTextStyle);

        Config.AutoAddComponents = EditorGUILayout.ToggleLeft(
            "<color=white><i>Auto add components to created objects</i></color>",
            Config.AutoAddComponents,
            UnityRichTextStyle);

        EditorGUILayout.Space();

        EditorGUILayout.Space();
        GUILayout.Label($"<size=20><color=yellow>Output path: <b>{Config.ExportPath}</b></color></size>", UnityRichTextStyle);
        if (GUI.Button(
                new Rect(10, 150, 200, 30),
                "<size=15><color=white><i>Change output directory</i></color></size>",
                new GUIStyle(GUI.skin.button)
                {
                    richText = true
                }))
        {
            string path = EditorUtility.OpenFolderPanel("Select output path", Config.ExportPath, "");

            if (!string.IsNullOrEmpty(path))
                Config.ExportPath = path;
        }

        if (GUI.Button(
                new Rect(225, 150, 200, 30),
                "<size=15><color=white><i>Reset output directory</i></color></size>",
                new GUIStyle(GUI.skin.button)
                {
                    richText = true
                }))
            Config.ExportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MapEditorReborn_CompiledSchematics");

        if (Config != _prevConfig)
        {
            _prevConfig = _prevConfig.CopyProperties(Config);
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(Config, Formatting.Indented));
        }
        
        GUILayout.EndArea();
    }

    public static GUIStyle UnityRichTextStyle
    {
        get
        {
            if (_settingsStyle != null)
                return _settingsStyle;

            _settingsStyle = new GUIStyle
            {
                richText = true,
            };

            return _settingsStyle;
        }
    }

    private static GUIStyle _settingsStyle;
    private static Config _prevConfig;
}