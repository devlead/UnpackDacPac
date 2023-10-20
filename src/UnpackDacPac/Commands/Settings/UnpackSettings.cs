using Cake.Core.IO;
using System.ComponentModel;

namespace UnpackDacPac.Commands.Settings;

public class UnpackSettings : CommandSettings
{
    private static readonly DirectoryPath CurrentDirectoryPath = Environment.CurrentDirectory;
    private FilePath dacPacPath = string.Empty;
    private DirectoryPath outputPath = CurrentDirectoryPath.FullPath;

    [CommandArgument(0, "<dacPacPath>")]
    [ValidatePath]
    [Description("DAC Package Path")]
    public FilePath DacPacPath
    {
        get => dacPacPath;
        set => dacPacPath = MakeAbsolute(value);
    }
    [CommandArgument(1, "<outputPath>")]
    [ValidatePath]
    [Description("Output path")]
    public DirectoryPath OutputPath
    {
        get => outputPath;
        set => outputPath = MakeAbsolute(value);
    }

    [CommandOption("--clean-output-path")]
    [Description("Flag for if output path should be cleaned before extraction.")]
    public bool CleanOutputPath { get; set; }

    [CommandOption("--target-database")]
    [Description("Target database name used for deploy script, if not specified DAC Package name without extension is used.")]
    public string? TargetDatabase {  get; set; }

    [CommandOption("--deploy-create-new-database")]
    [Description("Flag that specifies whether the existing database will be dropped and a new database created before proceeding with the actual deployment actions. Acquires single-user mode before dropping the existing database (default false).")]
    public bool CreateNewDatabase
    {
        get => DacDeployOptions.CreateNewDatabase;
        set => DacDeployOptions.CreateNewDatabase = value;
    }

    [CommandOption("--deploy-script-database-options")]
    [Description("Deployment script flag that specifies whether the database options in the target database should be updated to match the source model (default false).")]
    public bool ScriptDatabaseOptions
    {
        get => DacDeployOptions.ScriptDatabaseOptions;
        set => DacDeployOptions.ScriptDatabaseOptions = value;
    }

    [CommandOption("--deploy-script-ignore-permissions")]
    [Description("Deployment script flag that specifies whether to exclude all permission statements from consideration when comparing the source and target model (default false).")]
    public bool IgnorePermissions
    {
        get => DacDeployOptions.IgnorePermissions;
        set => DacDeployOptions.IgnorePermissions = value;
    }

    [CommandOption("--deploy-script-exclude-object-type <excludedObjectType>")]
    [Description("Deployment script setting to exclude object types")]
    public ObjectType[] ExcludeObjectTypes
    {
        get => DacDeployOptions.ExcludeObjectTypes;
        set => DacDeployOptions.ExcludeObjectTypes = value;
    }

    public DacDeployOptions DacDeployOptions { get; init; } = new DacDeployOptions
    {
        CreateNewDatabase = false,
        BlockWhenDriftDetected = false,
        IgnorePermissions = false,
        IgnoreRoleMembership = false,
        ExcludeObjectTypes = Array.Empty<ObjectType>()
    };


    private static FilePath MakeAbsolute(FilePath filePath)
    {
        return filePath.IsRelative 
            ? CurrentDirectoryPath.CombineWithFilePath(filePath)
            : filePath;
    }

    private static DirectoryPath MakeAbsolute(DirectoryPath directoryPath)
    {
        return directoryPath.IsRelative
            ? CurrentDirectoryPath.Combine(directoryPath)
            : directoryPath;
    }

    public override ValidationResult Validate()
    {
        if (
            !CleanOutputPath
            &&
            Directory.Exists(OutputPath.FullPath)
            &&
            Directory.EnumerateFileSystemEntries(OutputPath.FullPath, "*", SearchOption.AllDirectories).Any()
            )
        {
            return ValidationResult.Error($"OutputPath {OutputPath} not empty and CleanOutputPath ({CleanOutputPath}) not specified.");
        }
        return base.Validate();
    }
}