namespace Avro.Notion.Api.Notion;

/// <summary>
/// Central place to map meaningful names to Notion database/data source identifiers.
/// Add as many mappings as you need and reference them throughout the API.
/// </summary>
public static class NotionDatabaseConstants
{
    public static readonly NotionDataSourceMapping DefaultNotes = new(
        DatabaseId: "SET_DATABASE_ID",
        DataSourceId: "SET_DATA_SOURCE_ID");

    public static readonly NotionDataSourceMapping ArchivedNotes = new(
        DatabaseId: "SET_ARCHIVED_DATABASE_ID",
        DataSourceId: "SET_ARCHIVED_DATA_SOURCE_ID");

    public static readonly NotionDataSourceMapping Tasks = new(
        DatabaseId: "SET_TASKS_DATABASE_ID",
        DataSourceId: "SET_TASKS_DATA_SOURCE_ID");

    public static readonly NotionDataSourceMapping Projects = new(
        DatabaseId: "2b4f418e66f3800a9891cc7b1663e71f",
        DataSourceId: "SET_PROJECTS_DATA_SOURCE_ID");
}

public sealed record NotionDataSourceMapping(string DatabaseId, string DataSourceId);
