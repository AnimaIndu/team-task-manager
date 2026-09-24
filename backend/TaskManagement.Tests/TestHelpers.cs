using System.Net.Http.Headers;
using System.Net.Http.Json;

public record AuthUser(int Id, string Name, string Email, string Role, int? TeamId);
public record AuthResponse(string Token, AuthUser User);

public static class Roles
{
    public const int Admin = 0;
    public const int Manager = 1;
    public const int User = 2;
}

public static class TestHelpers
{
    // Registers a brand-new user with the given role and returns a client that
    // already has that user's token attached.
    public static async Task<(HttpClient Client, AuthResponse Auth)> CreateUserClientAsync(
        CustomFactory factory, int role)
    {
        var client = factory.CreateClient();
        var email = $"test{Guid.NewGuid():N}@test.com";

        var reg = await client.PostAsJsonAsync("/api/auth/register",
            new { name = "Test User", email, password = "password123", role });
        reg.EnsureSuccessStatusCode();

        var auth = (await reg.Content.ReadFromJsonAsync<AuthResponse>())!;
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", auth.Token);

        return (client, auth);
    }
}
