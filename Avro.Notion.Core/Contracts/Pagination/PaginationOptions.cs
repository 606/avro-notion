namespace Avro.Notion.Core.Contracts.Pagination;

public sealed class PaginationOptions
{
    private const int MinPageSize = 1;
    private const int MaxPageSize = 100;
    public const int DefaultPageSize = 25;

    public int PageSize { get; }
    public string? StartCursor { get; }

    public PaginationOptions(int pageSize = DefaultPageSize, string? startCursor = null)
    {
        if (pageSize is < MinPageSize or > MaxPageSize)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), $"Page size must be between {MinPageSize} and {MaxPageSize}.");
        }

        PageSize = pageSize;
        StartCursor = startCursor;
    }
}
