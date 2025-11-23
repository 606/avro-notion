using Avro.Notion.Core.Abstractions;
using Avro.Notion.Core.Domain.Entities;
using Avro.Notion.Core.Domain.ValueObjects;

namespace Avro.Notion.Core.Notes.Commands.UpdateNote;

public sealed class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, Note>
{
    private readonly INoteGateway _noteGateway;

    public UpdateNoteCommandHandler(INoteGateway noteGateway)
    {
        _noteGateway = noteGateway ?? throw new ArgumentNullException(nameof(noteGateway));
    }

    public async Task<Note> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NoteId))
        {
            throw new ArgumentException("Note identifier is required", nameof(request.NoteId));
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Title is required", nameof(request.Title));
        }

        var draft = NoteDraft.Create(request.Title, request.Content, request.Properties);
        return await _noteGateway.UpdateAsync(request.NoteId, draft, cancellationToken).ConfigureAwait(false);
    }
}
