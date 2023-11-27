﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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

                        IEnumerable<Session> sessionsToAdd = SessionTests.InitialSessions.Values.Select(dto => dto.ToSession());
                        context.AddRange(sessionsToAdd);
                        context.SaveChanges();
                        Assert.Equal(SessionTests.InitialSessions.Count, context.Sessions.Count());
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
