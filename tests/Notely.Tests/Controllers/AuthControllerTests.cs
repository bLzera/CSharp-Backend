using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Notely.Api.DTOs.Auth;
using Notely.Tests.Common.Factories;
using Notely.Tests.Common.Fixtures;

namespace Notely.Tests.Controllers;

public class AuthControllerTests(IntegrationWebAppFactory factory)
    : IClassFixture<IntegrationWebAppFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Register_ValidRequest_Returns200WithToken()
    {
        var req = UserFactory.CreateRegisterRequest();

        var response = await _client.PostAsJsonAsync("/auth/register", req);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns409()
    {
        var req = UserFactory.CreateRegisterRequest();
        await _client.PostAsJsonAsync("/auth/register", req);

        var response = await _client.PostAsJsonAsync("/auth/register", req);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithToken()
    {
        var req = UserFactory.CreateRegisterRequest();
        await _client.PostAsJsonAsync("/auth/register", req);

        var response = await _client.PostAsJsonAsync("/auth/login",
            UserFactory.CreateLoginRequest(req.Email));
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_InvalidPassword_Returns401()
    {
        var req = UserFactory.CreateRegisterRequest();
        await _client.PostAsJsonAsync("/auth/register", req);

        var response = await _client.PostAsJsonAsync("/auth/login",
            new LoginRequest(req.Email, "SenhaErrada123!"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
