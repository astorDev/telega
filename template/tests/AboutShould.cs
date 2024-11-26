namespace Telega.Tests;

[TestClass]
public class AboutShould : Test
{
    [TestMethod]
    public async Task ReturnValidMetadata()
    {
        var about = await this.Client.GetAbout();
        about.Should().BeEquivalentTo(new About(
            "Telega",
            "1.0.0.0",
            "Development",
            new Dictionary<string, object> {
                ["botClient"] = new { 
                    connected = true,
                    username = "TelegaBot"
                }
            }
        ), but => but.Excluding(a => a.Dependencies));
    }
}