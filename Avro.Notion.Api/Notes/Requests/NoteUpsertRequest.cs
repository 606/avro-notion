using System.ComponentModel.DataAnnotations;

namespace Avro.Notion.Api.Notes.Requests;

public sealed class NoteUpsertRequest
{
    [Required]
    [StringLength(256, MinimumLength = 1)]
    public string Title { get; init; } = string.Empty;

    [StringLength(2000)]
    public string? Content { get; init; }

    public Dictionary<string, string>? Properties { get; init; }
}
