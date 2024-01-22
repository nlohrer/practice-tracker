using Microsoft.EntityFrameworkCore;
using PracticeTrackerAPI.Models;
using PracticeTrackerAPI.Models.Session;

namespace PracticeTrackerAPI.Services
{
    public class SessionService : ISessionService
    {
        private readonly SessionContext _context;
        private readonly ISummaryService _summaryService;

        public SessionService(SessionContext context, ISummaryService summaryService)
        {
            _context = context;
            _summaryService = summaryService;
        }

        /// <inheritdoc />
        public async Task<Session?> FindSession(int id)
        {
            return await _context.Sessions.FindAsync(id);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SessionResponse>> GetSessions(string? username)
        {
            return await _context.Sessions
                .Where(session => username == null ? session.Username == null : session.Username == username)
                .Select(session => session.ToResponse())
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<(int id, SessionResponse sessionAsResponse)> AddSession(SessionDTO session, string? username)
        {
            Session asSession = session.ToSession();
            asSession.Username = username;

            _context.Sessions.Add(asSession);
            await _context.SaveChangesAsync();

            return ((int)asSession.Id, asSession.ToResponse());
        }

        /// <inheritdoc />
        public async Task<bool> UpdateSession(int id, Session session, SessionDTO sessionDTO)
        {
            session.Task = sessionDTO.Task;
            session.Duration = new TimeSpan(sessionDTO.Duration.Hours, sessionDTO.Duration.Minutes, 0);
            session.Date = sessionDTO.Date;
            session.Time = sessionDTO.Time;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SessionExists(id))
                {
                    return false;
                } else
                {
                    throw;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public async Task RemoveSession(Session session)
        {
            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<SessionSummary> GetSessionSummary(string username)
        {
            return await _summaryService.GetSessionSummary(username, _context);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SessionDTO>> SearchSessions(SessionSearch searchObject)
        {
            List<SessionDTO> searchResults = await _context.Sessions
                .Where(session => searchObject.Task != null ? EF.Functions.ILike(session.Task, $"%{searchObject.Task}%") : true)
                .Where(session => searchObject.DateFrom != null ? session.Date >= searchObject.DateFrom : true)
                .Where(session => searchObject.DateTo != null ? session.Date <= searchObject.DateTo : true)
                .Select(session => session.ToDTO())
                .ToListAsync();
            return searchResults;
        }

        /// <summary>
        /// Checks whether a session with the given <paramref name="id"/> exists in the database.
        /// </summary>
        /// <param name="id">The <paramref name="id"/> of the session.</param>
        /// <returns>true if a session with the given <paramref name="id"/> exists in the database; otherwise, false.</returns>
        private bool SessionExists(int id)
        {
            return _context.Sessions.Any(e => e.Id == id);
        }
    }
}