using Avro.Notion.Core.Abstractions;
using Avro.Notion.Infrastructure.Notion;
using Avro.Notion.Infrastructure.Options;
using Avro.Notion.Infrastructure.Pagination;
using Avro.Notion.Infrastructure.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Avro.Notion.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNotionInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.Configure<NotionOptions>(configuration.GetSection(NotionOptions.SectionName));

        services.AddSingleton<INotionRateLimiter>(static provider =>
        {
            var options = provider.GetRequiredService<IOptions<NotionOptions>>().Value;
            return new NotionRateLimiter(options.RateLimit);
        });

        services.AddSingleton<INotionPaginator, NotionPaginator>();

        services.AddHttpClient<INoteGateway, NotionNoteGateway>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<NotionOptions>>().Value;
            client.BaseAddress = new Uri(options.ApiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        });

        return services;
    }
}
