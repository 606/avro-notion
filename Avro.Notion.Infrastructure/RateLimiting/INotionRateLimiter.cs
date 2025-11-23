namespace Avro.Notion.Infrastructure.RateLimiting;

public interface INotionRateLimiter
{
    Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken);

    Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken);
}
