using System.Net;
using System.Net.Http.Json;
using Xunit;

public class AuthTests : IClassFixture<CustomFactory>
{
    private readonly HttpClient _client;

    public AuthTests(CustomFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_Then_Login_Succeeds()
    {
        var email = $"test{Guid.NewGuid()}@test.com";
        var reg = await _client.PostAsJsonAsync("/api/auth/register",
            new { name = "Test User", email, password = "password123", role = 2 });
       // Assert.Equal(HttpStatusCode.OK, reg.StatusCode);

        var body = await reg.Content.ReadAsStringAsync();
Assert.True(reg.StatusCode == HttpStatusCode.OK, $"Status: {reg.StatusCode}, Body: {body}");

        var login = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = "password123" });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
    }

    [Fact]
    public async Task Login_With_Wrong_Password_Returns_401()
    {
        var email = $"test{Guid.NewGuid()}@test.com";
        await _client.PostAsJsonAsync("/api/auth/register",
            new { name = "Test User", email, password = "password123", role = 2 });

        var login = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = "wrong" });
        Assert.Equal(HttpStatusCode.Unauthorized, login.StatusCode);
    }

    [Fact]
    public async Task Protected_Route_Without_Token_Returns_401()
    {
        var res = await _client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
public async Task User_Cannot_Do_Admin_Action()
{
    var (client, _) = await TestHelpers.CreateUserClientAsync(_factory, Roles.User);
    var res = await client.DeleteAsync("/api/tasks/1");   // <-- your admin-only endpoint
    Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
}

[Fact]
public async Task Admin_Can_Do_Admin_Action()
{
    var (client, _) = await TestHelpers.CreateUserClientAsync(_factory, Roles.Admin);
    var res = await client.DeleteAsync("/api/tasks/1");   // <-- same endpoint
    Assert.NotEqual(HttpStatusCode.Forbidden, res.StatusCode);
}
}
