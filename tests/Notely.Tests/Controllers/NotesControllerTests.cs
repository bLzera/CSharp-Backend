using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Notely.Api.DTOs.Notes;
using Notely.Tests.Common.Factories;
using Notely.Tests.Common.Fixtures;

namespace Notely.Tests.Controllers;

public class NotesControllerTests(IntegrationWebAppFactory factory)
    : IClassFixture<IntegrationWebAppFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_Authenticated_Returns200()
    {
        var token = await AuthHelper.RegisterAndGetTokenAsync(_client);

        var response = await _client.WithJwt(token).GetAsync("/notes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/notes");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_ValidRequest_Returns201WithLocation()
    {
        var token = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var req = NoteFactory.CreateRequest();

        var response = await _client.WithJwt(token).PostAsJsonAsync("/notes", req);
        var body = await response.Content.ReadFromJsonAsync<NoteResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        body!.Title.Should().Be(req.Title);
    }

    [Fact]
    public async Task GetById_ExistingNote_Returns200()
    {
        var token = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var created = await CreateNoteAsync(token);

        var response = await _client.WithJwt(token).GetAsync($"/notes/{created.Id}");
        var body = await response.Content.ReadFromJsonAsync<NoteResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetById_NoteOfOtherUser_Returns404()
    {
        var token1 = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var token2 = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var note = await CreateNoteAsync(token1);

        var response = await _client.WithJwt(token2).GetAsync($"/notes/{note.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_NonExistent_Returns404()
    {
        var token = await AuthHelper.RegisterAndGetTokenAsync(_client);

        var response = await _client.WithJwt(token).GetAsync($"/notes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_OwnNote_Returns200()
    {
        var token = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var note = await CreateNoteAsync(token);
        var req = NoteFactory.UpdateRequest();

        var response = await _client.WithJwt(token).PutAsJsonAsync($"/notes/{note.Id}", req);
        var body = await response.Content.ReadFromJsonAsync<NoteResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.Title.Should().Be(req.Title);
    }

    [Fact]
    public async Task Update_NoteOfOtherUser_Returns404()
    {
        var token1 = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var token2 = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var note = await CreateNoteAsync(token1);

        var response = await _client.WithJwt(token2).PutAsJsonAsync($"/notes/{note.Id}", NoteFactory.UpdateRequest());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_OwnNote_Returns204()
    {
        var token = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var note = await CreateNoteAsync(token);

        var response = await _client.WithJwt(token).DeleteAsync($"/notes/{note.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_NoteOfOtherUser_Returns404()
    {
        var token1 = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var token2 = await AuthHelper.RegisterAndGetTokenAsync(_client);
        var note = await CreateNoteAsync(token1);

        var response = await _client.WithJwt(token2).DeleteAsync($"/notes/{note.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<NoteResponse> CreateNoteAsync(string token)
    {
        var response = await _client.WithJwt(token).PostAsJsonAsync("/notes", NoteFactory.CreateRequest());
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<NoteResponse>())!;
    }
}
