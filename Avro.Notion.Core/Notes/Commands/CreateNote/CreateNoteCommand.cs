using Avro.Notion.Core.Domain.Entities;

namespace Avro.Notion.Core.Notes.Commands.CreateNote;

public sealed record CreateNoteCommand(
    string Title,
    string? Content,
    IReadOnlyDictionary<string, string>? Properties
) : IRequest<Note>;
