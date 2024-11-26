using Astor.Logging;
using Scalar.AspNetCore;
using Fluenv;
using Nist.Logs;
using Nist.Errors;
using Telegram.Bot.Types;
using Telegram.Bot;
using Confi;
using dotenv.net;

DotEnv.Load(new(envFilePaths: [ ".env", "../.env"] ));
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddFluentEnvironmentVariables();

builder.Services.AddOpenApi();
builder.Services.AddHttpClient<ITelegramBotClient, TelegramBotClient>(cl => {
    var token = builder.Configuration.GetRequiredValue("Telegram:Token");
    return new(token, cl);
});

builder.Logging.ClearProviders();
builder.Logging.AddMiniJsonConsole();
builder.Logging.AddSimpleConsole(c => c.SingleLine = true);

builder.Host.UseContentRoot(Directory.GetCurrentDirectory());

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(s => s.WithTheme(ScalarTheme.DeepSpace));

app.UseHttpIOLogging(l => l.Message = HttpIOMessagesRegistry.DefaultWithJsonBodies);
app.UseErrorBody(ex => ex switch {
    _ => Errors.Unknown
});

app.MapGet($"/{Uris.About}", async (IHostEnvironment env, ITelegramBotClient telegramBotClient) => new About(
    Description: "Telega",
    Version: typeof(Program).Assembly!.GetName().Version!.ToString(),
    Environment: env.EnvironmentName,
    Dependencies: new Dictionary<string, object> {
        ["botClient"] = new { 
            connected = true,
            username = (await telegramBotClient.GetMe())!.Username
        }
    }
));

app.MapPost($"/{Uris.Webhook}", async (Update update, ITelegramBotClient telegramBotClient) => {
    var chatId = (update.Message?.Chat.Id) ?? throw new ("Chat ID is not found in the update.");
    
    var sent = await telegramBotClient.SendMessage(
        chatId: chatId,
        text: "I don't quite get you, I'm here just to say hello. Hello!"
    );

    return sent;
});

app.Run();

public partial class Program;