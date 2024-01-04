using PracticeTrackerAPI.Models;
using PracticeTrackerAPI.Models.Session;

namespace PracticeTrackerAPI.Services
{
    public class SummaryService : ISummaryService
    {
        public SessionSummary GetSessionSummary(string username, SessionContext context)
        {
            return new SessionSummary();
        }
    }
}
