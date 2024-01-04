using PracticeTrackerAPI.Models;
using PracticeTrackerAPI.Models.Session;

namespace PracticeTrackerAPI.Services
{
    public interface ISummaryService
    {
        SessionSummary GetSessionSummary(string username, SessionContext context);
    }
}
