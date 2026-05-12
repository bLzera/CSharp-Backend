using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Notely.Api.Data;
using Notely.Api.Services;
using Notely.Tests.Common.Factories;

namespace Notely.Tests.Services;

public class AuthServiceTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static IConfiguration CreateConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "testing-secret-key-must-be-at-least-32-chars!!",
                ["Jwt:Issuer"] = "notely-api",
                ["Jwt:Audience"] = "notely-client",
                ["Jwt:ExpiresHours"] = "1",
                ["Jwt:RefreshTokenExpiryDays"] = "7"
            })
            .Build();

    [Fact]
    public async Task Register_WithNewEmail_ReturnsToken()
    {
        using var db = CreateDb();
        var service = new AuthService(db, CreateConfig());
        var req = UserFactory.CreateRegisterRequest();

        var result = await service.RegisterAsync(req);

        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsNull()
    {
        using var db = CreateDb();
        var service = new AuthService(db, CreateConfig());
        var req = UserFactory.CreateRegisterRequest();

        await service.RegisterAsync(req);
        var result = await service.RegisterAsync(req);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Register_StoresPasswordAsHash()
    {
        using var db = CreateDb();
        var service = new AuthService(db, CreateConfig());
        var req = UserFactory.CreateRegisterRequest();

        await service.RegisterAsync(req);

        var user = await db.Users.FirstAsync();
        user.PasswordHash.Should().NotBe(req.Password);
        BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        using var db = CreateDb();
        var service = new AuthService(db, CreateConfig());
        var registerReq = UserFactory.CreateRegisterRequest();
        await service.RegisterAsync(registerReq);

        var result = await service.LoginAsync(UserFactory.CreateLoginRequest(registerReq.Email));

        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsNull()
    {
        using var db = CreateDb();
        var service = new AuthService(db, CreateConfig());
        var registerReq = UserFactory.CreateRegisterRequest();
        await service.RegisterAsync(registerReq);

        var result = await service.LoginAsync(new(registerReq.Email, "WrongPassword!"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ReturnsNull()
    {
        using var db = CreateDb();
        var service = new AuthService(db, CreateConfig());

        var result = await service.LoginAsync(new("naoexiste@email.com", "Password123!"));

        result.Should().BeNull();
    }
}
