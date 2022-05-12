using App.WindowsService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

//https://docs.microsoft.com/pt-br/dotnet/core/extensions/windows-service

using IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = ".NET Joke Service";
    })
    .ConfigureServices((hostCtx, services) =>
    {
        var config = hostCtx.Configuration.GetSection("WorkerConfig");
        services.Configure<WorkerConfig>(config);
        services.AddSingleton<JokeService>();
        services.AddHostedService<WindowsBackgroundService>();
        services.AddHttpClient();
    })
    .Build();

await host.RunAsync();