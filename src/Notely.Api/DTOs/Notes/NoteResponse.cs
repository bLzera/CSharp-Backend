namespace Notely.Api.DTOs.Notes;

public record NoteResponse(
    Guid Id,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
