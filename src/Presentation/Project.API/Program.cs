using Project.Application.Extensions;
using Project.Infrastructure.Extensions;
using Project.Persistance.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddPersistence(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();
