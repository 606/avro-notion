using Avro.Notion.Core.Contracts.Pagination;

namespace Avro.Notion.Infrastructure.Pagination;

internal interface INotionPaginator
{
    Task<PaginatedList<T>> CreatePageAsync<T>(
        Func<string?, int, CancellationToken, Task<NotionPaginatedResponse<T>>> pageFactory,
        PaginationOptions options,
        CancellationToken cancellationToken);

    IAsyncEnumerable<T> EnumerateAsync<T>(
        Func<string?, int, CancellationToken, Task<NotionPaginatedResponse<T>>> pageFactory,
        PaginationOptions options,
        CancellationToken cancellationToken);
}
