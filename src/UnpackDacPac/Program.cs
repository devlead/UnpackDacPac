using UnpackDacPac.Commands;
using Spectre.Console.Cli.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection()
    .AddCakeCore()
    .AddLogging(configure =>
            configure
                .AddSimpleConsole(opts =>
                {
                    opts.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
                }))
    .AddSingleton<UnpackCommand>();


using var registrar = new DependencyInjectionRegistrar(serviceCollection);
var app = new CommandApp(registrar);

app.Configure(
    config =>
    {
        config.SetApplicationName("unpackdacpac");

        config.ValidateExamples();

        config.AddCommand<UnpackCommand>("unpack")
                .WithDescription("Example unpack command.")
                .WithExample(new[] { "unpack", "Test.dacpac", "outputpath" });
    });

return await app.RunAsync(args);