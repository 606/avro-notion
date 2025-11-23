using Avro.Notion.Core.Contracts.Pagination;
using Avro.Notion.Core.Domain.Entities;

namespace Avro.Notion.Core.Notes.Queries.ListNotes;

public sealed record ListNotesQuery(
    int PageSize = PaginationOptions.DefaultPageSize,
    string? StartCursor = null
) : IRequest<PaginatedList<NoteSummary>>
{
    public PaginationOptions ToPaginationOptions() => new(PageSize, StartCursor);
}
