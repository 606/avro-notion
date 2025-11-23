using Avro.Notion.Core.Domain.Entities;

namespace Avro.Notion.Core.Notes.Queries.GetNoteById;

public sealed record GetNoteByIdQuery(string NoteId) : IRequest<Note?>;
