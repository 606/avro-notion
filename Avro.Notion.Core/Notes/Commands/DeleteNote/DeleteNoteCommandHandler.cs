using Avro.Notion.Core.Abstractions;

namespace Avro.Notion.Core.Notes.Commands.DeleteNote;

public sealed class DeleteNoteCommandHandler(INoteGateway noteGateway) : IRequestHandler<DeleteNoteCommand, Unit>
{
    private readonly INoteGateway _noteGateway = noteGateway ?? throw new ArgumentNullException(nameof(noteGateway));

    public async Task<Unit> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NoteId))
        {
            throw new ArgumentException("Note identifier is required", nameof(request.NoteId));
        }

        await _noteGateway.DeleteAsync(request.NoteId, cancellationToken).ConfigureAwait(false);
        return Unit.Value;
    }
}
