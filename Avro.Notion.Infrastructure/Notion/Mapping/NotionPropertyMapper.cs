using System.Linq;
using Avro.Notion.Core.Domain.Entities;
using Avro.Notion.Infrastructure.Notion.Models;
using Avro.Notion.Infrastructure.Options;

namespace Avro.Notion.Infrastructure.Notion.Mapping;

internal static class NotionPropertyMapper
{
    public static Note ToNote(NotionPageResponse page, NotionOptions options)
    {
        var title = ExtractPlainText(page, options.TitlePropertyName) ?? string.Empty;
        var content = ExtractPlainText(page, options.ContentPropertyName);
        var properties = ExtractAdditionalProperties(page, options);

        return new Note(
            page.Id,
            title,
            content,
            properties,
            page.CreatedTime,
            page.LastEditedTime
        );
    }

    public static NoteSummary ToSummary(NotionPageResponse page, NotionOptions options)
    {
        var title = ExtractPlainText(page, options.TitlePropertyName) ?? string.Empty;
        var properties = ExtractAdditionalProperties(page, options);

        return new NoteSummary(
            page.Id,
            title,
            page.LastEditedTime,
            properties
        );
    }

    private static string? ExtractPlainText(NotionPageResponse page, string? propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return null;
        }

        if (!page.Properties.TryGetValue(propertyName, out var property))
        {
            return null;
        }

        var source = property.Type switch
        {
            "title" => property.Title,
            "rich_text" => property.RichText,
            _ => property.RichText ?? property.Title
        };

        return source?.FirstOrDefault()?.PlainText;
    }

    private static IReadOnlyDictionary<string, string>? ExtractAdditionalProperties(NotionPageResponse page, NotionOptions options)
    {
        if (page.Properties.Count == 0)
        {
            return null;
        }

        var excluded = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            options.TitlePropertyName,
            options.ContentPropertyName
        };

        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var (key, value) in page.Properties)
        {
            if (excluded.Contains(key))
            {
                continue;
            }

            var text = value.RichText ?? value.Title;
            if (text is null || text.Count == 0)
            {
                continue;
            }

            var plain = text[0].PlainText;
            if (string.IsNullOrWhiteSpace(plain))
            {
                continue;
            }

            result[key] = plain;
        }

        return result.Count == 0 ? null : result;
    }
}
