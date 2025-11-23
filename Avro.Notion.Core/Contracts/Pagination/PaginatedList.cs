namespace Avro.Notion.Core.Contracts.Pagination;

public sealed class PaginatedList<T>(IReadOnlyList<T> items, string? nextCursor, bool hasMore)
{
    public IReadOnlyList<T> Items { get; } = items ?? throw new ArgumentNullException(nameof(items));

    public string? NextCursor { get; } = nextCursor;

    public bool HasMore { get; } = hasMore;

    public static PaginatedList<T> Empty() => new([], null, false);
}
