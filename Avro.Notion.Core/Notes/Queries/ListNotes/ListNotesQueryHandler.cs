using Avro.Notion.Core.Abstractions;
using Avro.Notion.Core.Contracts.Pagination;
using Avro.Notion.Core.Domain.Entities;

namespace Avro.Notion.Core.Notes.Queries.ListNotes;

public sealed class ListNotesQueryHandler(INoteGateway noteGateway)
    : IRequestHandler<ListNotesQuery, PaginatedList<NoteSummary>>
{
    private readonly INoteGateway _noteGateway = noteGateway ?? throw new ArgumentNullException(nameof(noteGateway));

    public Task<PaginatedList<NoteSummary>> Handle(ListNotesQuery request, CancellationToken cancellationToken)
    {
        var pagination = request.ToPaginationOptions();
        return _noteGateway.SearchAsync(pagination, cancellationToken);
    }
}
