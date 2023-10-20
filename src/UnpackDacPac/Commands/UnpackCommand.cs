using Cake.Common.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace UnpackDacPac.Commands;

public class UnpackCommand : AsyncCommand<UnpackSettings>
{
    private ICakeContext CakeContext { get; }
    private ILogger Logger { get; }

    public override async Task<int> ExecuteAsync(CommandContext context, UnpackSettings settings)
    {
        Logger.LogInformation("DacPacPath: {DacPacPath}", settings.DacPacPath);
        Logger.LogInformation("OutputPath: {OutputPath}", settings.OutputPath);

        var dacPacPath = settings.DacPacPath;
        var targetPath = settings.OutputPath;
        var targetDatabaseName = settings.TargetDatabase ?? settings.DacPacPath.GetFilenameWithoutExtension().FullPath;
        var deployScriptPath = targetPath.CombineWithFilePath("Deploy.sql");

        CakeContext.EnsureDirectoryExists(targetPath);

        if (settings.CleanOutputPath)
        {
            CakeContext.CleanDirectory(targetPath);
        }

        using var dacpacStream = CakeContext.FileSystem.GetFile(settings.DacPacPath).OpenRead();
        
        Logger.LogInformation("Loading DAC Package {DacPacPath}...", dacPacPath);
        using var dacpac = DacPackage.Load(dacpacStream, DacSchemaModelStorageType.Memory, FileAccess.Read);
        

        Logger.LogInformation(
            """
            Opened DAC Package {DacPacPath}
                Name:           {Name}
                Version:        {Version}
                Description:    {Description}
            """,
            dacPacPath,
            dacpac.Name,
            dacpac.Version,
            dacpac.Description
            );

        Logger.LogInformation("Unpacking DAC Package to {TargetPath}...", targetPath);
        dacpac.Unpack(targetPath.FullPath);
        

        foreach (var file in CakeContext.GetFiles($"{targetPath}/**/*.*"))
        {
            Logger.LogInformation("{FilePath}", settings.OutputPath.GetRelativePath(file));
        }

        Logger.LogInformation("Generating deploy script...");

        using var deployScriptStream = CakeContext.FileSystem.GetFile(deployScriptPath).OpenWrite();

        DacServices.GenerateCreateScript(
            deployScriptStream,
            dacpac,
            targetDatabaseName,
            settings.DacDeployOptions
            );

        Logger.LogInformation("{FilePath}", settings.OutputPath.GetRelativePath(deployScriptPath));

        Logger.LogInformation("Done.");

        return await Task.FromResult(0);
    }

    public UnpackCommand(
        ICakeContext cakeContext,
        ILogger<UnpackCommand> logger
        )
    {
        CakeContext = cakeContext;
        Logger = logger;
    }
}