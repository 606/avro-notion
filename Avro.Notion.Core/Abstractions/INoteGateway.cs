using Avro.Notion.Core.Contracts.Pagination;
using Avro.Notion.Core.Domain.Entities;
using Avro.Notion.Core.Domain.ValueObjects;

namespace Avro.Notion.Core.Abstractions;

public interface INoteGateway
{
    Task<Note> CreateAsync(NoteDraft draft, CancellationToken cancellationToken);

    Task<Note?> GetAsync(string noteId, CancellationToken cancellationToken);

    Task<Note> UpdateAsync(string noteId, NoteDraft draft, CancellationToken cancellationToken);

    Task DeleteAsync(string noteId, CancellationToken cancellationToken);

    Task<PaginatedList<NoteSummary>> SearchAsync(PaginationOptions options, CancellationToken cancellationToken);
}
