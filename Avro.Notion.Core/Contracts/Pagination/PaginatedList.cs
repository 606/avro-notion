namespace Avro.Notion.Core.Contracts.Pagination;

public sealed class PaginatedList<T>
{
    public PaginatedList(IReadOnlyList<T> items, string? nextCursor, bool hasMore)
    {
        Items = items ?? throw new ArgumentNullException(nameof(items));
        NextCursor = nextCursor;
        HasMore = hasMore;
    }

    public IReadOnlyList<T> Items { get; }

    public string? NextCursor { get; }

    public bool HasMore { get; }

    public static PaginatedList<T> Empty() => new(Array.Empty<T>(), null, false);
}
