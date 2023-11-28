using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PracticeTrackerAPI.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

string? connectionString;
if (builder.Environment.IsProduction())
{
    builder.Configuration.AddEnvironmentVariables();
    connectionString = builder.Configuration.GetValue<string>("POSTGRES_CONNECTION_STRING");
}
else
{
    connectionString = builder.Configuration.GetConnectionString("Pgsql");
}
builder.Services.AddDbContext<SessionContext>(opt =>
    opt.UseNpgsql(connectionString: connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "Practice Tracker",
        Description = "An API for tracking practice sessions",
        Version = "1.0"
    });
    string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.MapType<DateOnly>(() => new OpenApiSchema { Type = "string" });
    options.MapType<TimeOnly>(() => new OpenApiSchema { Type = "string" });
});


var app = builder.Build();
if (app.Environment.IsProduction())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<SessionContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
