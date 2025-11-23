namespace Avro.Notion.Core.Domain.Entities;

public sealed record NoteSummary(
    string Id,
    string Title,
    DateTimeOffset LastEditedAt,
    IReadOnlyDictionary<string, string>? Properties
);
