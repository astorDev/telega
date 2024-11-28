using Telegram.Bot;

namespace Telega.TelegramModels;

public record Response(
    string MessageText
) {
    public static implicit operator Response(string message) => new(message);
}

public record Context(
    long ChatId,
    string? TextMessage
) {
    public static Context From(Telegram.Bot.Types.Update update)
    {
        if (update.Message == null)
        {
            throw new("Unable to process update without message");
        }

        return new(
            ChatId: update.Message.Chat.Id,
            TextMessage: update.Message.Text
        );
    }
}

public static class TelegramBotClientExtensions
{
    public static async Task<Response> Send(this ITelegramBotClient client, Response response, Context context)
    {
        await client.SendMessage(context.ChatId, response.MessageText);
        return response;
    }
}