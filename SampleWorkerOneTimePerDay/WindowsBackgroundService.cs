using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace App.WindowsService;

public sealed class WindowsBackgroundService : BackgroundService
{
    private readonly JokeService _jokeService;
    private readonly ILogger<WindowsBackgroundService> _logger;
    private readonly WorkerConfig _config;
    private readonly HttpClient _client;


    public WindowsBackgroundService(JokeService jokeService,
        ILogger<WindowsBackgroundService> logger,
        IOptions<WorkerConfig> config,
        HttpClient client) =>
        (_jokeService, _logger, _config, _client) = (jokeService, logger, config.Value, client);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var minutes = TimeSpan.FromMinutes(FirstTime(_config.WorkerStartTime));
        await Task.Delay(minutes, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _client.GetFromJsonAsync<ChuckNorrisJoke>("https://api.chucknorris.io/jokes/random");
            string joke = _jokeService.GetJoke();
            _logger.LogWarning(joke);

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private static double FirstTime(string? WorkerStartTime)
    {

        var date = Convert.ToDateTime(WorkerStartTime);
        var diff = date.Subtract(DateTime.Now);
        
        if (diff.TotalMinutes >= 0D)
            return diff.TotalMinutes;

        diff = date.AddDays(1).Subtract(DateTime.Now);
        return diff.TotalMinutes;

        //return 0;
    }
}

public record ChuckNorrisJoke(string Id, string Value, string Url);