namespace Avro.Notion.Core.Domain.Entities;

public sealed record Note(
    string Id,
    string Title,
    string? Content,
    IReadOnlyDictionary<string, string>? Properties,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
