using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telega;

public class LongPoller(
    IServiceScopeFactory scopeFactory,
    ILogger<LongPoller> logger) : BackgroundService
{
    private int offset;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            await PollOnce(stoppingToken);
    }

    public async Task PollOnce(CancellationToken stoppingToken)
    {
        try { await PollAndPauseIfIdle(stoppingToken); }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
        catch (Exception exception) { await LogAndPause(exception, stoppingToken); }
    }

    public async Task PollAndPauseIfIdle(CancellationToken stoppingToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var pollService = scope.ServiceProvider.GetRequiredService<PollService>();
        var updates = await pollService.Run(offset, stoppingToken);
        if (updates.Length > 0) offset = updates[^1].Id + 1;
        if (updates.Length == 0) await Task.Delay(TimeSpan.FromMilliseconds(500), stoppingToken);
    }

    public async Task LogAndPause(Exception exception, CancellationToken stoppingToken)
    {
        logger.LogError(exception, "Long polling failed");
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
    }
}

public class PollService(
    ITelegramBotClient telegramBotClient,
    UpdateProcessor processor,
    ILogger<PollService> logger)
{
    public async Task<Update[]> Run(int offset, CancellationToken stoppingToken)
    {
        logger.LogTrace("Starting long-poll run at offset {Offset}", offset);
        var updates = await telegramBotClient.GetUpdates(offset, timeout: 10, cancellationToken: stoppingToken);
        foreach (var update in updates)
            await ProcessUpdate(update);
        logger.LogDebug(
            "Completed long-poll run with offset {Offset}. Received {UpdatesCount} updates.",
            offset,
            updates.Length);
        return updates;
    }

    public Task<Response> ProcessUpdate(Update update) => processor.Search(update);
}

public static class LongPollingExtensions
{
    public static IServiceCollection AddLongPollingIfTurnedOn(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (configuration["Mode"] == "LongPolling") RegisterLongPolling(services);
        return services;
    }

    public static void RegisterLongPolling(IServiceCollection services)
    {
        services.AddScoped<PollService>();
        services.AddHostedService<LongPoller>();
    }
}
