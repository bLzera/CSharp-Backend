using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Notely.Api.DTOs.NoteGroups;
using Notely.Tests.Common.Factories;
using Notely.Tests.Common.Fixtures;

namespace Notely.Tests.Controllers;

public class NoteGroupsControllerTests(IntegrationWebAppFactory factory)
    : IClassFixture<IntegrationWebAppFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_Authenticated_Returns200()
    {
        var token = await AuthHelper.RegisterAndGetTokenAsync(_client);

        var response = await _client.WithJwt(token).GetAsync("/note-groups");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/note-groups");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_ValidRequest_Returns201WithLocation()
    {
        var token = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var req = NoteGroupFactory.CreateRequest();

        var response = await _client.WithJwt(token).PostAsJsonAsync("/note-groups", req);
        var body = await response.Content.ReadFromJsonAsync<NoteGroupResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        body!.Name.Should().Be(req.Name);
    }

    [Fact]
    public async Task GetById_ExistingGroup_Returns200()
    {
        var token = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var group = await CreateGroupAsync(token);

        var response = await _client.WithJwt(token).GetAsync($"/note-groups/{group.Id}");
        var body = await response.Content.ReadFromJsonAsync<NoteGroupResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.Id.Should().Be(group.Id);
    }

    [Fact]
    public async Task GetById_GroupOfOtherUser_Returns404()
    {
        var token1 = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var token2 = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var group = await CreateGroupAsync(token1);

        var response = await _client.WithJwt(token2).GetAsync($"/note-groups/{group.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_OwnGroup_Returns200()
    {
        var token = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var group = await CreateGroupAsync(token);
        var req = NoteGroupFactory.UpdateRequest();

        var response = await _client.WithJwt(token).PutAsJsonAsync($"/note-groups/{group.Id}", req);
        var body = await response.Content.ReadFromJsonAsync<NoteGroupResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.Name.Should().Be(req.Name);
    }

    [Fact]
    public async Task Update_GroupOfOtherUser_Returns404()
    {
        var token1 = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var token2 = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var group = await CreateGroupAsync(token1);

        var response = await _client.WithJwt(token2)
            .PutAsJsonAsync($"/note-groups/{group.Id}", NoteGroupFactory.UpdateRequest());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_OwnGroup_Returns204()
    {
        var token = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var group = await CreateGroupAsync(token);

        var response = await _client.WithJwt(token).DeleteAsync($"/note-groups/{group.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_GroupOfOtherUser_Returns404()
    {
        var token1 = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var token2 = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var group = await CreateGroupAsync(token1);

        var response = await _client.WithJwt(token2).DeleteAsync($"/note-groups/{group.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<NoteGroupResponse> CreateGroupAsync(string token)
    {
        var response = await _client.WithJwt(token)
            .PostAsJsonAsync("/note-groups", NoteGroupFactory.CreateRequest());
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<NoteGroupResponse>())!;
    }
}
