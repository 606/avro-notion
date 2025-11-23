using Avro.Notion.Api.Filters;
using Avro.Notion.Api.Notion;
using Avro.Notion.Core.Notes.Commands.CreateNote;
using Avro.Notion.Infrastructure.DependencyInjection;
using Avro.Notion.Infrastructure.Options;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services.AddControllers(options =>
    options.Filters.Add<HttpRequestExceptionFilter>());
builder.Services.AddOpenApi();
builder.Services.AddNotionInfrastructure(builder.Configuration);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateNoteCommand>());
builder.Services.PostConfigure<NotionOptions>(options =>
{
    var mapping = NotionDatabaseConstants.DefaultNotes;
    options.DatabaseId = mapping.DatabaseId;
    options.DataSourceId = mapping.DataSourceId;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Avro Notion API";
        options.Theme = ScalarTheme.DeepSpace;
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
