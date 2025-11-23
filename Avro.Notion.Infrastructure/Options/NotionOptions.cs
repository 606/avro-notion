using Avro.Notion.Infrastructure.RateLimiting;

namespace Avro.Notion.Infrastructure.Options;

public sealed class NotionOptions
{
    public const string SectionName = "Notion";

    public string ApiBaseUrl { get; init; } = "https://api.notion.com";

    public string ApiVersion { get; init; } = "2025-09-03";

    public string IntegrationToken { get; init; } = string.Empty;

    public string DatabaseId { get; set; } = string.Empty;

    public string DataSourceId { get; set; } = string.Empty;

    public string TitlePropertyName { get; init; } = "Name";

    public string ContentPropertyName { get; init; } = "Content";

    public NotionRateLimitOptions RateLimit { get; init; } = new();

    public void Validate()
    {
        EnsureValuePresent(IntegrationToken, "Notion integration token is missing.");
        EnsureValuePresent(DatabaseId, "Notion database id is missing.");
        EnsureValuePresent(DataSourceId, "Notion data source id is missing.");

        if (LooksLikePlaceholder(IntegrationToken))
        {
            throw new InvalidOperationException("Notion integration token still uses the placeholder value. Provide a real integration secret.");
        }

        ValidateGuid(DatabaseId, "Notion database id");
        ValidateGuid(DataSourceId, "Notion data source id");
    }

    private static void EnsureValuePresent(string? value, string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(errorMessage);
        }
    }

    private static void ValidateGuid(string candidate, string friendlyName)
    {
        if (LooksLikePlaceholder(candidate) || !Guid.TryParse(candidate, out _))
        {
            throw new InvalidOperationException($"{friendlyName} is invalid or still uses a placeholder value. Update your configuration with the real identifier.");
        }
    }

    private static bool LooksLikePlaceholder(string value) => value.StartsWith("SET_", StringComparison.OrdinalIgnoreCase);
}
