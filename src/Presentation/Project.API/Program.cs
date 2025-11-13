var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAPIServices(builder.Configuration);

var app = builder.Build();

// Configure middleware pipeline
app.UseAPIMiddlewares();

app.Run();