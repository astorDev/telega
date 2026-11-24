using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Telega;

[TestClass]
public class DependenciesShould : Test
{
    [TestMethod]
    public async Task ResolveMediatorAndHandleStart()
    {
        var mediator = Factory.Services.GetRequiredService<IMediator>();
        var response = await mediator.Send(new Start());
        response.MessageText.ShouldBe("Hello! Thank you for starting");
    }

    [TestMethod]
    public async Task ServeOpenApi()
    {
        var client = Factory.CreateClient();
        var response = await client.GetAsync("/openapi/v1.json");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.ShouldBe(HttpStatusCode.OK, body);
    }
}
