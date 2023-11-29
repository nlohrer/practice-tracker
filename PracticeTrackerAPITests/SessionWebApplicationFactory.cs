using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PracticeTrackerAPI.Models;

namespace PracticeAPITests
{
    public class SessionWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var configuration = new ConfigurationBuilder();
            string? connectionString = configuration.AddJsonFile("appsettings.Tests.json").Build().GetConnectionString("Pgsql_Test");
            Assert.NotNull(connectionString);

            builder.ConfigureAppConfiguration((WebHostBuilderContext hostingContext, IConfigurationBuilder configurationBuilder) =>
            {
                configurationBuilder.AddJsonFile("appsettings.Tests.json", optional: false, reloadOnChange: true);
            });

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<SessionContext>>();
                services.AddDbContext<SessionContext>(opt =>
                    opt.UseNpgsql(connectionString));
            });
        }
    }
}
