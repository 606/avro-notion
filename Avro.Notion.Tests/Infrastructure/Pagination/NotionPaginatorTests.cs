using Avro.Notion.Core.Contracts.Pagination;
using Avro.Notion.Infrastructure.Pagination;
using FluentAssertions;

namespace Avro.Notion.Tests.Infrastructure.Pagination;

public sealed class NotionPaginatorTests
{
    private readonly NotionPaginator _paginator = new();

    [Fact]
    public async Task CreatePageAsync_ReturnsPaginatedListFromFactory()
    {
        // Arrange
        var options = new PaginationOptions(pageSize: 5, startCursor: "abc");
        var expectedItems = new[] { "a", "b" };
        var expectedResponse = new NotionPaginatedResponse<string>(expectedItems, "next", true);

        Task<NotionPaginatedResponse<string>> Factory(string? cursor, int pageSize, CancellationToken ct)
        {
            cursor.Should().Be(options.StartCursor);
            pageSize.Should().Be(options.PageSize);
            return Task.FromResult(expectedResponse);
        }

        // Act
        var page = await _paginator.CreatePageAsync(Factory, options, CancellationToken.None);

        // Assert
        page.Items.Should().BeEquivalentTo(expectedItems);
        page.NextCursor.Should().Be("next");
        page.HasMore.Should().BeTrue();
    }

    [Fact]
    public async Task EnumerateAsync_YieldsAllPagesSequentially()
    {
        var options = new PaginationOptions(pageSize: 2);
        var responses = new Queue<NotionPaginatedResponse<int>>();
        responses.Enqueue(new NotionPaginatedResponse<int>(new[] { 1, 2 }, "cursor-1", true));
        responses.Enqueue(new NotionPaginatedResponse<int>(new[] { 3 }, null, false));

        Task<NotionPaginatedResponse<int>> Factory(string? cursor, int pageSize, CancellationToken ct)
        {
            pageSize.Should().Be(options.PageSize);
            return Task.FromResult(responses.Dequeue());
        }

        var collected = new List<int>();
        await foreach (var item in _paginator.EnumerateAsync(Factory, options, CancellationToken.None))
        {
            collected.Add(item);
        }

        collected.Should().ContainInOrder(1, 2, 3);
    }
}
