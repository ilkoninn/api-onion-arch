using Project.Application;
using Project.Infrastructure;
using Project.Persistance;
using Project.Persistance.Contexts;
using Project.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add dependency injection services
builder.Services.AddPersistanceDependencyInjection(builder.Configuration);
builder.Services.AddSignalRDependencyInjection(builder.Configuration);
builder.Services.AddInfrastructureDependencyInjection();
builder.Services.AddApplicationDependencyInjection();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Add admin user and roles to the database
using var scope = app.Services.CreateScope();
await AutomatedMigration.MigrateAsync(scope.ServiceProvider);

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();
