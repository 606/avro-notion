using Avro.Notion.Api.Notion;
using Avro.Notion.Core.Notes.Commands.CreateNote;
using Avro.Notion.Infrastructure.DependencyInjection;
using Avro.Notion.Infrastructure.Options;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddNotionInfrastructure(builder.Configuration);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateNoteCommand>());
builder.Services.PostConfigure<NotionOptions>(options =>
{
    options.DatabaseId = NotionDatabaseConstants.DefaultNotes;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
