namespace Avro.Notion.Infrastructure.RateLimiting;

public sealed class NotionRateLimitOptions
{
    public int RequestsPerInterval { get; init; } = 3;

    public TimeSpan Interval { get; init; } = TimeSpan.FromSeconds(1);

    public int BurstCapacity { get; init; } = 6;

    public int QueueLimit { get; init; } = 50;
}
