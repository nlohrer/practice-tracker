using PracticeTrackerAPI.Models.Session;

namespace PracticeTrackerAPI.Services
{
    public interface ISessionService
    {
        /// <summary>
        /// Fetches all sessions associated with the provided user.
        /// </summary>
        /// <param name="username">The name of the user.</param>
        /// <returns>The list of sessions belonging to the provided user.</returns>
        Task<IEnumerable<SessionResponse>> GetSessions(string? username);

        /// <summary>
        /// Finds a session by <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The <paramref name="id"/> of the session.</param>
        /// <returns>The session with the provided <paramref name="id"/>.</returns>
        Task<Session?> FindSession(int id);

        /// <summary>
        /// Adds a session for a user.
        /// </summary>
        /// <param name="session">The session that should be added.</param>
        /// <param name="username">The name of the user.</param>
        /// <returns>A tuple containing the id of the new session as well as a SessionResponse object representing the newly added session.</returns>
        Task<(int id, SessionResponse sessionAsResponse)> AddSession(SessionDTO session, string? username);

        /// <summary>
        /// Removes a session.
        /// </summary>
        /// <param name="session">The session to remove.</param>
        Task RemoveSession(Session session);

        /// <summary>
        /// Updates a session.
        /// </summary>
        /// <param name="id">The id of the session.</param>
        /// <param name="session">The original session.</param>
        /// <param name="sessionDTO">The new session.</param>
        /// <returns>true if the id was found in the database; false otherwise.</returns>
        Task<bool> UpdateSession(int id, Session session, SessionDTO sessionDTO);

        /// <summary>
        /// Searches the database for fitting session.
        /// </summary>
        /// <param name="searchObject">A SessionSearch object representing the search parameters.</param>
        /// <returns>A list of sessions matching the provided parameters.</returns>
        Task<IEnumerable<SessionDTO>> SearchSessions(SessionSearch searchObject);

        /// <summary>
        /// Gets a summary for a the provided user's sessions.
        /// </summary>
        /// <param name="username">The name of the user.</param>
        /// <returns>A SessionSummary object representing the summary.</returns>
        Task<SessionSummary> GetSessionSummary(string username);
    }
}