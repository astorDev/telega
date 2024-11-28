global using Telega.TelegramModels;
global using MediatR;
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

builder.Services.AddPipeline<Context, IRequest<Response>>(o => o
    .Add(Start.Recognize)
    .Add(Unrecognized.Instance)
);

builder.Services.AddMediatR(m => m.RegisterServicesFromAssemblyContaining<Program>());

builder.Logging.ClearProviders();
builder.Logging.AddMiniJsonConsole();
builder.Logging.AddSimpleConsole(c => c.SingleLine = true);

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
    Dependencies: new() {
        ["botClient"] = new { 
            connected = true,
            username = (await telegramBotClient.GetMe()).Username
        }
    }
));

app.MapPost($"/{Uris.Webhook}", async (Update update, ITelegramBotClient telegramBotClient, Pipeline<Context, IRequest<Response>> pipeline, IMediator mediator) => {
    var context = Context.From(update);

    var command = await pipeline.Process(context);
    if (command == null) throw new("Unable to process the request");

    var response = await mediator.Send(command);

    return await telegramBotClient.Send(response, context);
});

app.Run();

public partial class Program;