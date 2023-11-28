using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeTrackerAPI.Models;

namespace PracticeTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SessionController : ControllerBase
    {
        private readonly SessionContext _context;

        public SessionController(SessionContext context)
        {
            _context = context;
        }

        /// <summary>
        /// List all practice sessions.
        /// </summary>
        /// <returns>A list of all practice sessions.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/Session
        /// </remarks>
        /// <response code="200">Returns all practice sessions.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SessionDTO>>> GetSessions()
        {
            return await _context.Sessions.Select(session => session.ToDTO()).ToListAsync();
        }

        /// <summary>
        /// Get a specific practice session by <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The <paramref name="id"/> of the practice session.</param>
        /// <returns>The practice session with the specified <paramref name="id"/>.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/Session/1
        /// </remarks>
        /// <response code="200">Returns the requested practice session.</response>
        /// <response code="404">If no practice session matches the given <paramref name="id"/>.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SessionDTO>> GetSession(int id)
        {
            Session? session = await _context.Sessions.FindAsync(id);

            if (session == null)
            {
                return NotFound();
            }

            return Ok(session.ToDTO());
        }

        /// <summary>
        /// Update a specific practice session.
        /// </summary>
        /// <param name="id">The <paramref name="id"/> of the session that you want to update.</param>
        /// <param name="sessionDTO">The new session.</param>
        /// <returns>Nothing.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/Session/1
        ///     {
        ///         "task": "learn math",
        ///         "duration": {
        ///             "hours": 1,
        ///             "minutes": 50
        ///         },
        ///         "date": "2022-02-26",
        ///         "time": "14:05:25"
        ///     }
        /// </remarks>
        /// <response code="204">If the update was successful.</response>
        /// <response code="400">If the new session could not be validated.</response>
        /// <response code="404">If no practice session matches the given <paramref name="id"/>.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        /// <summary>
        /// Create a new practice session.
        /// </summary>
        /// <param name="session">A JSON object representing the session.</param>
        /// <returns>The newly created session.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/Session
        ///     {
        ///         "task": "practice clarinet",
        ///         "duration": {
        ///             "hours": 1,
        ///             "minutes": 30
        ///          },
        ///          "date": "2024-07-03",
        ///          "time": "18:15:00"
        ///      }
        /// </remarks>
        /// <response code="201">Returns the newly created item.</response>
        /// <response code="400">If the practice session could not be validated.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SessionDTO>> PostSession(SessionDTO session)
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

        /// <summary>
        /// Deletes a specific practice session by id.
        /// </summary>
        /// <param name="id">The <paramref name="id"/> of the practice session.</param>
        /// <returns>Nothing.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /api/Session/1
        /// </remarks>
        /// <response code="204">If the practice session was deleted successfully</response>
        /// <response code="404">If no practice session matched the given <paramref name="id"/>.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Searches the database for fitting sessions.
        /// </summary>
        /// <param name="searchObject">An object representing the parameters for the search.</param>
        /// <returns>A list of sessions that matched the given parameters.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/Session/Search
        ///     {
        ///         "task": "practice"
        ///     }
        /// </remarks>
        /// <response code="200">Even if no matching practice session was found. In that case, the returned list is empty.</response>
        [HttpPost("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
        }
    }
}
