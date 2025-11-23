using Avro.Notion.Infrastructure.RateLimiting;

namespace Avro.Notion.Infrastructure.Options;

public sealed class NotionOptions
{
    public const string SectionName = "Notion";

    public string ApiBaseUrl { get; init; } = "https://api.notion.com";

    public string ApiVersion { get; init; } = "2022-06-28";

    public string IntegrationToken { get; init; } = string.Empty;

    public string DatabaseId { get; set; } = string.Empty;

    public string TitlePropertyName { get; init; } = "Name";

    public string ContentPropertyName { get; init; } = "Content";

    public NotionRateLimitOptions RateLimit { get; init; } = new();

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(IntegrationToken))
        {
            throw new InvalidOperationException("Notion integration token is missing.");
        }

        if (string.IsNullOrWhiteSpace(DatabaseId))
        {
            throw new InvalidOperationException("Notion database id is missing.");
        }
    }
}
