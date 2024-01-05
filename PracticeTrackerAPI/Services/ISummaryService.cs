using PracticeTrackerAPI.Models;
using PracticeTrackerAPI.Models.Session;

namespace PracticeTrackerAPI.Services
{
    public interface ISummaryService
    {
        /// <summary>
        /// Query a session summary from the database.
        /// </summary>
        /// <param name="username">The <paramref name="username"/> of the user who's practice sessions should be summarized.</param>
        /// <param name="context">The database context for the user's sessions.</param>
        /// <returns>A summary of the provided user's practice sessions.</returns>
        Task<SessionSummary> GetSessionSummary(string username, SessionContext context);
    }
}
