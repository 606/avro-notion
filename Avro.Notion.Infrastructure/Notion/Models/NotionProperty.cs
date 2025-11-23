using System.Text.Json.Serialization;

namespace Avro.Notion.Infrastructure.Notion.Models;

internal sealed class NotionProperty
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("title")]
    public List<NotionRichText>? Title { get; init; }

    [JsonPropertyName("rich_text")]
    public List<NotionRichText>? RichText { get; init; }
}
