namespace Avro.Notion.Core.Domain.ValueObjects;

public sealed record NoteDraft(
    string Title,
    string? Content,
    IReadOnlyDictionary<string, string>? Properties
)
{
    public static NoteDraft Create(string title, string? content = null, IReadOnlyDictionary<string, string>? properties = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required", nameof(title));
        }

        return new NoteDraft(title.Trim(), content, properties);
    }
}
