using System.Runtime.CompilerServices;
using Avro.Notion.Core.Contracts.Pagination;

namespace Avro.Notion.Infrastructure.Pagination;

internal sealed class NotionPaginator : INotionPaginator
{
    public async Task<PaginatedList<T>> CreatePageAsync<T>(
        Func<string?, int, CancellationToken, Task<NotionPaginatedResponse<T>>> pageFactory,
        PaginationOptions options,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pageFactory);
        ArgumentNullException.ThrowIfNull(options);

        var response = await pageFactory(options.StartCursor, options.PageSize, cancellationToken).ConfigureAwait(false);
        return new PaginatedList<T>(response.Results, response.NextCursor, response.HasMore);
    }

    public async IAsyncEnumerable<T> EnumerateAsync<T>(
        Func<string?, int, CancellationToken, Task<NotionPaginatedResponse<T>>> pageFactory,
        PaginationOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pageFactory);
        ArgumentNullException.ThrowIfNull(options);

        var cursor = options.StartCursor;
        do
        {
            var response = await pageFactory(cursor, options.PageSize, cancellationToken).ConfigureAwait(false);

            foreach (var item in response.Results)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return item;
            }

            cursor = response.NextCursor;

            if (!response.HasMore)
            {
                yield break;
            }
        }
        while (true);
    }
}
