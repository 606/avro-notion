using Avro.Notion.Core.Abstractions;
using Avro.Notion.Core.Notes.Commands.DeleteNote;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace Avro.Notion.Tests.Core.Notes.Commands;

public sealed class DeleteNoteCommandHandlerTests
{
    private readonly INoteGateway _noteGateway = Substitute.For<INoteGateway>();
    private readonly DeleteNoteCommandHandler _handler;

    public DeleteNoteCommandHandlerTests()
    {
        _handler = new DeleteNoteCommandHandler(_noteGateway);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_InvokesGatewayAndReturnsUnit()
    {
        var command = new DeleteNoteCommand("page-123");

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Should().Be(Unit.Value);
        await _noteGateway
            .Received(1)
            .DeleteAsync(command.NoteId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNoteIdMissing_ThrowsArgumentException()
    {
        var command = new DeleteNoteCommand("  ");

        await FluentActions
            .Invoking(() => _handler.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*Note identifier*");
    }
}
