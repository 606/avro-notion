using Avro.Notion.Core.Contracts.Pagination;
using Avro.Notion.Core.Domain.Entities;
using Avro.Notion.Core.Notes.Commands.CreateNote;
using Avro.Notion.Core.Notes.Commands.DeleteNote;
using Avro.Notion.Core.Notes.Commands.UpdateNote;
using Avro.Notion.Core.Notes.Queries.GetNoteById;
using Avro.Notion.Core.Notes.Queries.ListNotes;
using Avro.Notion.Api.Notes.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Avro.Notion.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class NotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotesController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<NoteSummary>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<NoteSummary>>> GetNotesAsync(
        [FromQuery] int pageSize = PaginationOptions.DefaultPageSize,
        [FromQuery] string? startCursor = null,
        CancellationToken cancellationToken = default)
    {
        var notes = await _mediator.Send(new ListNotesQuery(pageSize, startCursor), cancellationToken).ConfigureAwait(false);
        return Ok(notes);
    }

    [HttpGet("{noteId}")]
    [ProducesResponseType(typeof(Note), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Note>> GetNoteAsync(string noteId, CancellationToken cancellationToken)
    {
        var note = await _mediator.Send(new GetNoteByIdQuery(noteId), cancellationToken).ConfigureAwait(false);
        if (note is null)
        {
            return NotFound();
        }

        return Ok(note);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Note), StatusCodes.Status201Created)]
    public async Task<ActionResult<Note>> CreateNoteAsync([FromBody] NoteUpsertRequest request, CancellationToken cancellationToken)
    {
        var note = await _mediator.Send(
            new CreateNoteCommand(request.Title, request.Content, request.Properties),
            cancellationToken).ConfigureAwait(false);

        return CreatedAtAction(nameof(GetNoteAsync), new { noteId = note.Id }, note);
    }

    [HttpPut("{noteId}")]
    [ProducesResponseType(typeof(Note), StatusCodes.Status200OK)]
    public async Task<ActionResult<Note>> UpdateNoteAsync(string noteId, [FromBody] NoteUpsertRequest request, CancellationToken cancellationToken)
    {
        var note = await _mediator.Send(
            new UpdateNoteCommand(noteId, request.Title, request.Content, request.Properties),
            cancellationToken).ConfigureAwait(false);

        return Ok(note);
    }

    [HttpDelete("{noteId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteNoteAsync(string noteId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteNoteCommand(noteId), cancellationToken).ConfigureAwait(false);
        return NoContent();
    }
}
