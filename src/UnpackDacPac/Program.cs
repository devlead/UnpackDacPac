using UnpackDacPac.Commands;

public partial class Program
{
    static partial void AddServices(IServiceCollection services)
    {
        services
            .AddCakeCore();
    }

    static partial void ConfigureApp(AppServiceConfig appServiceConfig)
    {
        appServiceConfig.SetApplicationName("unpackdacpac");


        appServiceConfig.AddCommand<UnpackCommand>("unpack")
                .WithDescription("Example unpack command.")
                .WithExample(["unpack", "Test.dacpac", "outputpath" ]);
    }
}