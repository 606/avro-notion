using System.Text.Json.Serialization;

namespace Avro.Notion.Infrastructure.Notion.Models;

internal sealed class NotionPageResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("created_time")]
    public DateTimeOffset CreatedTime { get; init; }

    [JsonPropertyName("last_edited_time")]
    public DateTimeOffset LastEditedTime { get; init; }

    [JsonPropertyName("properties")]
    public Dictionary<string, NotionProperty> Properties { get; init; } = new();
}
