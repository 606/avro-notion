using Avro.Notion.Core.Domain.Entities;

namespace Avro.Notion.Core.Notes.Commands.UpdateNote;

public sealed record UpdateNoteCommand(
    string NoteId,
    string Title,
    string? Content,
    IReadOnlyDictionary<string, string>? Properties
) : IRequest<Note>;
