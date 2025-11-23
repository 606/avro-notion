using System.Text.Json.Serialization;

namespace Avro.Notion.Infrastructure.Notion.Models;

internal sealed class NotionQueryResponse
{
    [JsonPropertyName("results")]
    public List<NotionPageResponse> Results { get; init; } = new();

    [JsonPropertyName("has_more")]
    public bool HasMore { get; init; }

    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; init; }
}
