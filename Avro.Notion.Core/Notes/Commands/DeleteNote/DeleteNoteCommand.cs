namespace Avro.Notion.Core.Notes.Commands.DeleteNote;

public sealed record DeleteNoteCommand(string NoteId) : IRequest<Unit>;
