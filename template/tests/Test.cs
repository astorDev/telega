global using FakeItEasy;
global using Telegram.Bot;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


public class Test
{
    public static readonly ITelegramBotClient BotClient = A.Fake<ITelegramBotClient>();
    protected WebApplicationFactory Factory { get; } =  new();
    protected Client Client { get;  }

    protected Test()
    {
        var services = new ServiceCollection();
        services.AddLogging(l => l.AddSimpleConsole(c => c.SingleLine = true));
        var logger = services.BuildServiceProvider().GetRequiredService<ILogger<Telega.Client>>();
        
        Client = new(Factory.CreateClient(), logger);
    }
    
    public class WebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(s =>
            {
                s.AddSingleton(BotClient);
            });
            
            base.ConfigureWebHost(builder);
        }
    }
}