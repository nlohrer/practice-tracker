using Microsoft.EntityFrameworkCore;

namespace PracticeTrackerAPI.Models
{
    public class SessionContext : DbContext
    {
        public SessionContext(DbContextOptions<SessionContext> options) : base(options) { }

        public DbSet<Session.Session> Sessions { get; set; } = null!;

        public DbSet<User.User> Users { get; set; } = null!;
    }
}
