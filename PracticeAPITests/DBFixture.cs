using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PracticeTrackerAPI.Models;

namespace PracticeAPITests
{
    public class DBFixture
    {
        private static readonly Object _lock = new();
        private static bool _databaseInitialized;

        public DBFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (SessionContext context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        context.AddRange(
                            new Session(
                                id: 3, task: "play violin",
                                duration: TimeSpan.Parse("02:30:00"),
                                date: DateOnly.Parse("2020/02/15"),
                                time: TimeOnly.Parse("06:30")
                            ),
                            new Session(
                                id: 4, task: "learn math",
                                duration: TimeSpan.Parse("01:15:00"),
                                date: DateOnly.Parse("2021/09/03"),
                                time: TimeOnly.Parse("11:30")
                            )
                        );
                        context.SaveChanges();
                    }
                    _databaseInitialized = true;
                }
            }
        }

        public SessionContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SessionContext>();
            string? connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.Tests.json").Build().GetConnectionString("Pgsql_Test");
            DbContextOptions<SessionContext> options = optionsBuilder.UseNpgsql(connectionString).Options;
            return new SessionContext(options);
        }
    }

}
