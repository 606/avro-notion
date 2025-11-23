using Avro.Notion.Core.Abstractions;
using Avro.Notion.Core.Domain.Entities;

namespace Avro.Notion.Core.Notes.Queries.GetNoteById;

public sealed class GetNoteByIdQueryHandler : IRequestHandler<GetNoteByIdQuery, Note?>
{
    private readonly INoteGateway _noteGateway;

    public GetNoteByIdQueryHandler(INoteGateway noteGateway)
    {
        _noteGateway = noteGateway ?? throw new ArgumentNullException(nameof(noteGateway));
    }

    public Task<Note?> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NoteId))
        {
            throw new ArgumentException("Note identifier is required", nameof(request.NoteId));
        }

        return _noteGateway.GetAsync(request.NoteId, cancellationToken);
    }
}
