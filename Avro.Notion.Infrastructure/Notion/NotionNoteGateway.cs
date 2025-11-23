using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avro.Notion.Core.Abstractions;
using Avro.Notion.Core.Contracts.Pagination;
using Avro.Notion.Core.Domain.Entities;
using Avro.Notion.Core.Domain.ValueObjects;
using Avro.Notion.Infrastructure.Notion.Mapping;
using Avro.Notion.Infrastructure.Notion.Models;
using Avro.Notion.Infrastructure.Options;
using Avro.Notion.Infrastructure.Pagination;
using Avro.Notion.Infrastructure.RateLimiting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Avro.Notion.Infrastructure.Notion;

internal sealed class NotionNoteGateway : INoteGateway
{
    private static readonly Uri PagesEndpoint = new("/v1/pages", UriKind.Relative);

    private readonly HttpClient _httpClient;
    private readonly INotionRateLimiter _rateLimiter;
    private readonly INotionPaginator _paginator;
    private readonly NotionOptions _options;
    private readonly ILogger<NotionNoteGateway> _logger;
    private readonly JsonSerializerOptions _serializerOptions;

    public NotionNoteGateway(
        HttpClient httpClient,
        IOptions<NotionOptions> options,
        INotionRateLimiter rateLimiter,
        INotionPaginator paginator,
        ILogger<NotionNoteGateway> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _rateLimiter = rateLimiter ?? throw new ArgumentNullException(nameof(rateLimiter));
        _paginator = paginator ?? throw new ArgumentNullException(nameof(paginator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _options.Validate();

        _serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<Note> CreateAsync(NoteDraft draft, CancellationToken cancellationToken)
    {
        var payload = NotionPayloadFactory.BuildCreatePayload(draft, _options);
        var page = await SendAsync<NotionPageResponse>(HttpMethod.Post, PagesEndpoint, payload, cancellationToken).ConfigureAwait(false);
        return NotionPropertyMapper.ToNote(page, _options);
    }

    public async Task<Note?> GetAsync(string noteId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(noteId))
        {
            throw new ArgumentException("Note identifier is required", nameof(noteId));
        }

        try
        {
            var page = await SendAsync<NotionPageResponse>(HttpMethod.Get, BuildPageUri(noteId), payload: null, cancellationToken).ConfigureAwait(false);
            return NotionPropertyMapper.ToNote(page, _options);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogDebug(ex, "Notion page {NoteId} was not found", noteId);
            return null;
        }
    }

    public async Task<Note> UpdateAsync(string noteId, NoteDraft draft, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(noteId))
        {
            throw new ArgumentException("Note identifier is required", nameof(noteId));
        }

        var payload = NotionPayloadFactory.BuildUpdatePayload(draft, _options);
        var page = await SendAsync<NotionPageResponse>(HttpMethod.Patch, BuildPageUri(noteId), payload, cancellationToken).ConfigureAwait(false);
        return NotionPropertyMapper.ToNote(page, _options);
    }

    public Task DeleteAsync(string noteId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(noteId))
        {
            throw new ArgumentException("Note identifier is required", nameof(noteId));
        }

        var payload = NotionPayloadFactory.BuildArchivePayload();
        return SendAsync(HttpMethod.Patch, BuildPageUri(noteId), payload, cancellationToken);
    }

    public Task<PaginatedList<NoteSummary>> SearchAsync(PaginationOptions options, CancellationToken cancellationToken)
    {
        return _paginator.CreatePageAsync(async (cursor, pageSize, ct) =>
        {
            var payload = NotionPayloadFactory.BuildQueryPayload(pageSize, cursor);
            var response = await SendAsync<NotionQueryResponse>(HttpMethod.Post, BuildQueryUri(), payload, ct).ConfigureAwait(false);
            var summaries = response.Results
                .Select(page => NotionPropertyMapper.ToSummary(page, _options))
                .ToList();

            return new NotionPaginatedResponse<NoteSummary>(summaries, response.NextCursor, response.HasMore);
        }, options, cancellationToken);
    }

    private Uri BuildPageUri(string pageId) => new($"/v1/pages/{pageId}", UriKind.Relative);

    private Uri BuildQueryUri() => new($"/v1/data_sources/{_options.DataSourceId}/query", UriKind.Relative);

    private Task SendAsync(HttpMethod method, Uri uri, object? payload, CancellationToken cancellationToken)
        => _rateLimiter.ExecuteAsync(async ct =>
        {
            using var request = CreateRequest(method, uri, payload);
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
            await EnsureSuccess(response, ct).ConfigureAwait(false);
        }, cancellationToken);

    private Task<TResponse> SendAsync<TResponse>(HttpMethod method, Uri uri, object? payload, CancellationToken cancellationToken)
        => _rateLimiter.ExecuteAsync(async ct =>
        {
            using var request = CreateRequest(method, uri, payload);
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
            await EnsureSuccess(response, ct).ConfigureAwait(false);
            await using var stream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
            var result = await JsonSerializer.DeserializeAsync<TResponse>(stream, _serializerOptions, ct).ConfigureAwait(false);

            return result ?? throw new InvalidOperationException("Notion response could not be deserialized.");
        }, cancellationToken);

    private HttpRequestMessage CreateRequest(HttpMethod method, Uri uri, object? payload)
    {
        var request = new HttpRequestMessage(method, uri);

        if (payload is not null)
        {
            request.Content = JsonContent.Create(payload, options: _serializerOptions);
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.IntegrationToken);
        request.Headers.Add("Notion-Version", _options.ApiVersion);

        return request;
    }

    private async Task EnsureSuccess(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogError("Notion API responded with {StatusCode}: {Body}", (int)response.StatusCode, body);

        throw new HttpRequestException(
            $"Notion API request failed with {(int)response.StatusCode} ({response.ReasonPhrase}). Body: {body}",
            null,
            response.StatusCode);
    }
}
