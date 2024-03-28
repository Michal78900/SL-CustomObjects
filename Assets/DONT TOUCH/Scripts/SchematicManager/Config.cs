using System;
using System.IO;

public class Config
{
    public Config()
    {
    }

    public Config(Config other) =>
        CopyProperties(other);
    
    public Config CopyProperties(Config source)
    {
        OpenDirectoryAfterCompiling = source.OpenDirectoryAfterCompiling;
        ExportPath = source.ExportPath;
        ZipCompiledSchematics = source.ZipCompiledSchematics;
        AutoAddComponents = source.AutoAddComponents;

        return this;
    }

    public bool OpenDirectoryAfterCompiling { get; set; } = false;

    public string ExportPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MapEditorReborn_CompiledSchematics");

    public bool ZipCompiledSchematics { get; set; } = false;

    public bool AutoAddComponents { get; set; } = true;

    public static bool operator ==(Config config, Config other) =>
        config.OpenDirectoryAfterCompiling == other.OpenDirectoryAfterCompiling &&
        config.ExportPath == other.ExportPath &&
        config.ZipCompiledSchematics == other.ZipCompiledSchematics &&
        config.AutoAddComponents == other.AutoAddComponents;

    public static bool operator !=(Config config, Config other) => !(config == other);
}
