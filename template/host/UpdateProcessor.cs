using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telega;

public class UpdateProcessor(
    ITelegramBotClient telegramBotClient,
    Pipeline<Context, IRequest<Response>> pipeline,
    IMediator mediator,
    ILogger<UpdateProcessor> logger)
{
    public async Task<Response> Search(Update update)
    {
        var context = Context.From(update);
        logger.LogDebug("Processing update for chat {ChatId}", context.ChatId);
        var command = await pipeline.Process(context);
        if (command == null) throw new InvalidOperationException("No command for update was found");
        var response = await mediator.Send(command);
        var sentResponse = await telegramBotClient.Send(response, context);
        logger.LogInformation(
            "Sent {ResponseType} response to chat {ChatId}",
            command.GetType().Name,
            context.ChatId);
        return sentResponse;
    }
}
