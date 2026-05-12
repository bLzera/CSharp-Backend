using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notely.Api.DTOs.Notes;
using Notely.Api.Services;

namespace Notely.Api.Controllers;

[ApiController]
[Route("notes")]
[Authorize]
public class NotesController(NoteService noteService) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await noteService.GetAllAsync(UserId));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var note = await noteService.GetByIdAsync(UserId, id);
        return note is null ? NotFound() : Ok(note);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateNoteRequest req)
    {
        var note = await noteService.CreateAsync(UserId, req);
        return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateNoteRequest req)
    {
        var note = await noteService.UpdateAsync(UserId, id, req);
        return note is null ? NotFound() : Ok(note);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await noteService.DeleteAsync(UserId, id);
        return deleted ? NoContent() : NotFound();
    }
}
