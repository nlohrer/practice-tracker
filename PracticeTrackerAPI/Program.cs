using Microsoft.EntityFrameworkCore;
using PracticeTrackerAPI.Models;

var builder = WebApplication.CreateBuilder(args);

string? connectionString;
if (builder.Environment.IsProduction())
{
    builder.Configuration.AddEnvironmentVariables();
    connectionString = builder.Configuration.GetValue<string>("POSTGRES_CONNECTION_STRING");
} else
{
    connectionString = builder.Configuration.GetConnectionString("Pgsql");
}

builder.Services.AddControllers();
builder.Services.AddDbContext<SessionContext>(opt =>
    opt.UseNpgsql(connectionString: connectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
