using Avro.Notion.Core.Abstractions;
using Avro.Notion.Core.Domain.Entities;
using Avro.Notion.Core.Domain.ValueObjects;
using Avro.Notion.Core.Notes.Commands.CreateNote;
using FluentAssertions;
using NSubstitute;

namespace Avro.Notion.Tests.Core.Notes.Commands;

public sealed class CreateNoteCommandHandlerTests
{
    private readonly INoteGateway _noteGateway = Substitute.For<INoteGateway>();
    private readonly CreateNoteCommandHandler _handler;

    public CreateNoteCommandHandlerTests()
    {
        _handler = new CreateNoteCommandHandler(_noteGateway);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_PassesDraftToGateway()
    {
        // Arrange
        var command = new CreateNoteCommand("Meeting notes", "Discuss roadmap", new Dictionary<string, string>
        {
            ["status"] = "draft"
        });

        NoteDraft? capturedDraft = null;
        var expected = new Note("page-id", command.Title, command.Content, command.Properties, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

        _noteGateway
            .CreateAsync(Arg.Do<NoteDraft>(draft => capturedDraft = draft), Arg.Any<CancellationToken>())
            .Returns(expected);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(expected);
        capturedDraft.Should().NotBeNull();
        capturedDraft!.Title.Should().Be(command.Title);
        capturedDraft.Content.Should().Be(command.Content);
        capturedDraft.Properties.Should().BeEquivalentTo(command.Properties);
    }

    [Fact]
    public async Task Handle_WhenTitleMissing_ThrowsArgumentException()
    {
        var command = new CreateNoteCommand("   ", null, null);

        await FluentActions
            .Invoking(() => _handler.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*Title*");
    }
}
