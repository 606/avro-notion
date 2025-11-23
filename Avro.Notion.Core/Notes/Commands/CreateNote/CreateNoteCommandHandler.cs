using Avro.Notion.Core.Abstractions;
using Avro.Notion.Core.Domain.Entities;
using Avro.Notion.Core.Domain.ValueObjects;

namespace Avro.Notion.Core.Notes.Commands.CreateNote;

public sealed class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, Note>
{
    private readonly INoteGateway _noteGateway;

    public CreateNoteCommandHandler(INoteGateway noteGateway)
    {
        _noteGateway = noteGateway ?? throw new ArgumentNullException(nameof(noteGateway));
    }

    public async Task<Note> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Title is required", nameof(request.Title));
        }

        var draft = NoteDraft.Create(request.Title, request.Content, request.Properties);
        return await _noteGateway.CreateAsync(draft, cancellationToken).ConfigureAwait(false);
    }
}
