using System.Threading.RateLimiting;

namespace Avro.Notion.Infrastructure.RateLimiting;

public sealed class NotionRateLimiter : INotionRateLimiter, IAsyncDisposable
{
    private readonly RateLimiter _limiter;

    public NotionRateLimiter(NotionRateLimitOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _limiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            TokenLimit = Math.Max(options.BurstCapacity, options.RequestsPerInterval),
            TokensPerPeriod = options.RequestsPerInterval,
            ReplenishmentPeriod = options.Interval,
            AutoReplenishment = true,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = options.QueueLimit
        });
    }

    public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(action);
        while (true)
        {
            var lease = await _limiter.AcquireAsync(1, cancellationToken).ConfigureAwait(false);
            if (lease.IsAcquired)
            {
                using (lease)
                {
                    return await action(cancellationToken).ConfigureAwait(false);
                }
            }

            if (lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            {
                await Task.Delay(retryAfter, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    public async Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(action);
        await ExecuteAsync(async ct =>
        {
            await action(ct).ConfigureAwait(false);
            return true;
        }, cancellationToken).ConfigureAwait(false);
    }

    public ValueTask DisposeAsync() => _limiter.DisposeAsync();
}
