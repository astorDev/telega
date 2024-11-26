using Telegram.Bot.Types;

namespace Telega;

public partial class Uris {
    public const string Webhook = "webhook";
}

public partial interface IClient {
    Task<Message> PostWebhook(Update update);
}

public partial class Client {
    public async Task<Message> PostWebhook(Update update) => await Post<Message>(Uris.Webhook, update);
}