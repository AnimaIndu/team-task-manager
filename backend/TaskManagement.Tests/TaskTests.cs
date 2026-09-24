using System.Net;
using Xunit;

public class TaskTests : IClassFixture<CustomFactory>
{
    private readonly CustomFactory _factory;
    public TaskTests(CustomFactory factory) => _factory = factory;

    [Fact]
    public async Task Get_Tasks_Without_Token_Returns_401()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/api/tasks");   // <-- use your real route
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Get_Tasks_With_Token_Returns_200()
    {
        var (client, _) = await TestHelpers.CreateUserClientAsync(_factory, Roles.User);
        var res = await client.GetAsync("/api/tasks");   // <-- use your real route
        var body = await res.Content.ReadAsStringAsync();
        Assert.True(res.StatusCode == HttpStatusCode.OK, $"Status: {res.StatusCode}, Body: {body}");
    }
}
