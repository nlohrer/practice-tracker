using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeTrackerAPI.Models;

namespace PracticeTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly SessionContext _context;

        public SessionController(SessionContext context)
        {
            _context = context;
        }

        // GET: api/Session
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Session>>> GetSessions()
        {
            return await _context.Sessions.ToListAsync();
        }

        // GET: api/Session/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Session>> GetSession(int id)
        {
            Session? session = await _context.Sessions.FindAsync(id);

            if (session == null)
            {
                return NotFound();
            }

            return Ok(session.ToDTO());
        }

        // PUT: api/Session/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSession(int id, SessionDTO sessionDTO)
        {

            Session? session = await _context.Sessions.FindAsync(id);

            if (session is null)
            {
                return NotFound();
            }

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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Session
        [HttpPost]
        public async Task<ActionResult<Session>> PostSession(SessionDTO session)
        {
            _context.Sessions.Add(session.ToSession());
            await _context.SaveChangesAsync();
            if (ModelState.IsValid)
            {
                // Search for the freshly created object to get its id
                Session? addedSession = await _context.Sessions
                    .Where( found => found.Task == session.Task &&
                        found.Duration.Hours == session.Duration.Hours &&
                        found.Duration.Minutes == session.Duration.Minutes &&
                        found.Date == session.Date &&
                        found.Time == session.Time)
                    .FirstAsync();
                return CreatedAtAction("GetSession", new { id = addedSession.Id }, session);
            } else
            {
                return BadRequest(ModelState);
            }

        }

        // DELETE: api/Session/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSession(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Session/search
        /// <summary>
        /// Searches the database for fitting sessions.
        /// </summary>
        /// <param name="searchObject">An object representing the parameters for the search.</param>
        /// <returns>A list of sessions that matched the given parameters.</returns>
        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<SessionDTO>>> SearchSessions(SessionSearch searchObject)
        {
            var searchResults = await _context.Sessions
                .Where(session => session.Task.Contains(searchObject.Task))
                .Select(session => session.ToDTO())
                .ToListAsync();
            return Ok(searchResults);
        }

        /// <summary>
        /// Checks whether a session with the given <paramref name="id"/> exists in the database.
        /// </summary>
        /// <param name="id">The <paramref name="id"/> of the session.</param>
        /// <returns>true if a session with the given <paramref name="id"/> exists in the database; otherwise,false.</returns>
        private bool SessionExists(int id)
        {
            return _context.Sessions.Any(e => e.Id == id);
            "abc".IsNormalized();
        }
    }
}
