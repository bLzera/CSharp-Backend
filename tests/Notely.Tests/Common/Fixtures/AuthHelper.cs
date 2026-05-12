using System.Net.Http.Headers;
using System.Net.Http.Json;
using Notely.Api.DTOs.Auth;
using Notely.Tests.Common.Factories;

namespace Notely.Tests.Common.Fixtures;

public static class AuthHelper
{
    public static async Task<string> RegisterAndGetTokenAsync(
        HttpClient client,
        string? email = null)
    {
        var req = UserFactory.CreateRegisterRequest(email);
        var response = await client.PostAsJsonAsync("/auth/register", req);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return body!.AccessToken;
    }

    public static HttpClient WithJwt(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
