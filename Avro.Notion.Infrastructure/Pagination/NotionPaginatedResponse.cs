namespace Avro.Notion.Infrastructure.Pagination;

internal sealed record NotionPaginatedResponse<T>(
    IReadOnlyList<T> Results,
    string? NextCursor,
    bool HasMore
);
