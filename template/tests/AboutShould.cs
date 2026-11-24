namespace Telega;

[TestClass]
public class AboutShould : Test
{
    [TestMethod]
    public async Task ReturnValidMetadata()
    {
        var about = await this.Client.GetAbout();
        about.Description.ShouldBe("Telega");
        about.Version.ShouldBe("1.0.0.0");
        about.Environment.ShouldBe("Development");
    }
}