using System.Text.Json.Serialization;

namespace Avro.Notion.Infrastructure.Notion.Models;

internal sealed class NotionRichText
{
    [JsonPropertyName("plain_text")]
    public string PlainText { get; init; } = string.Empty;

    [JsonPropertyName("href")]
    public string? Href { get; init; }
}
