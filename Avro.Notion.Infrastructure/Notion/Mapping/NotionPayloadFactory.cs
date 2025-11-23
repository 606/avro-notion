using Avro.Notion.Core.Domain.ValueObjects;
using Avro.Notion.Infrastructure.Options;

namespace Avro.Notion.Infrastructure.Notion.Mapping;

internal static class NotionPayloadFactory
{
    public static object BuildCreatePayload(NoteDraft draft, NotionOptions options)
    {
        return new
        {
            parent = new { database_id = options.DatabaseId },
            properties = BuildProperties(draft, options)
        };
    }

    public static object BuildUpdatePayload(NoteDraft draft, NotionOptions options)
    {
        return new
        {
            properties = BuildProperties(draft, options)
        };
    }

    public static object BuildArchivePayload(bool archived = true)
    {
        return new
        {
            archived
        };
    }

    public static object BuildQueryPayload(int pageSize, string? startCursor)
    {
        return new
        {
            page_size = pageSize,
            start_cursor = startCursor
        };
    }

    private static IDictionary<string, object> BuildProperties(NoteDraft draft, NotionOptions options)
    {
        var properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            [options.TitlePropertyName] = BuildTitleProperty(draft.Title)
        };

        if (!string.IsNullOrWhiteSpace(draft.Content) && !string.IsNullOrWhiteSpace(options.ContentPropertyName))
        {
            properties[options.ContentPropertyName] = BuildRichTextProperty(draft.Content!);
        }

        if (draft.Properties is not null)
        {
            foreach (var (key, value) in draft.Properties)
            {
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                properties[key] = BuildRichTextProperty(value);
            }
        }

        return properties;
    }

    private static object BuildTitleProperty(string value)
    {
        return new
        {
            title = new[]
            {
                new
                {
                    text = new { content = value }
                }
            }
        };
    }

    private static object BuildRichTextProperty(string value)
    {
        return new
        {
            rich_text = new[]
            {
                new
                {
                    text = new { content = value }
                }
            }
        };
    }
}
