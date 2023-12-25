using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PracticeTrackerAPI.Models;
using PracticeTrackerAPI.Models.Session;
using PracticeTrackerAPI.Models.User;
using PracticeTrackerAPITests;

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

                        // Seed the database initially
                        IEnumerable<Session> sessionsToAdd = SessionTests.InitialSessions.Values.Select(dto => dto.ToSession());
                        IEnumerable<User> usersToAdd = UserTests.InitialUsers.Values;

                        context.AddRange(sessionsToAdd);
                        context.AddRange(usersToAdd);
                        context.SaveChanges();
                        Assert.Equal(SessionTests.InitialSessions.Count, context.Sessions.Count());
                    }
                    _databaseInitialized = true;
                }
            }
        }

        /// <summary>
        /// Creates a SessionContext using appsettings.Tests.json
        /// </summary>
        /// <returns>The SessionContext using the connection string from appsettings.Tests.json</returns>
        public SessionContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SessionContext>();
            string? connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.Tests.json").Build().GetConnectionString("Pgsql_Test");
            DbContextOptions<SessionContext> options = optionsBuilder.UseNpgsql(connectionString).Options;
            return new SessionContext(options);
        }
    }

}
