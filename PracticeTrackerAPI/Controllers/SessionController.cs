﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeTrackerAPI.Migrations;
using PracticeTrackerAPI.Models;
using PracticeTrackerAPI.Models.Session;
using PracticeTrackerAPI.Services;

namespace PracticeTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ResponseCache(Duration = 20)]
    [Produces("application/json")]
    public class SessionController : ControllerBase
    {
        private readonly SessionContext _context;

        public SessionController(SessionContext context)
        {
            _context = context;
        }

        /// <summary>
        /// List all practice sessions not assigned to any user. If a username is provided, list only the sessions of that user.
        /// </summary>
        /// <param name="username">The <paramref name="username"/> of the user</param>
        /// <returns>A list of all practice sessions.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/Session?username=user
        /// </remarks>
        /// <response code="200">Returns all practice sessions for the specified user. Returns all practice sessions with no assigned user instead if no username is specified</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SessionResponse>>> GetSessions([FromQuery] string? username)
        {
            return await _context.Sessions
                .Where(session => username == null ? session.Username == null : session.Username == username)
                .Select(session => session.ToResponse())
                .ToListAsync();
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
        public async Task<ActionResult<SessionResponse>> GetSession(int id)
        {
            Session? session = await _context.Sessions.FindAsync(id);

            if (session == null)
            {
                return NotFound();
            }

            return Ok(session.ToResponse());
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
        /// <param name="username">The user who did the session</param>
        /// <returns>The newly created session.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/Session?username=user
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
        public async Task<ActionResult<SessionResponse>> PostSession(SessionDTO session, string? username)
        {
            Session asSession = session.ToSession();
            asSession.Username = username;

            _context.Sessions.Add(asSession);
            await _context.SaveChangesAsync();
            if (ModelState.IsValid)
            {
                return CreatedAtAction("GetSession", new { id = asSession.Id }, asSession.ToResponse());
            }
            else
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
        /// Searches the database for fitting sessions case-insensitively.
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
            List<SessionDTO> searchResults = await _context.Sessions
                .Where(session => searchObject.Task != null ? EF.Functions.ILike(session.Task, $"%{searchObject.Task}%") : true)
                .Where(session => searchObject.DateFrom != null ? session.Date >= searchObject.DateFrom : true)
                .Where(session => searchObject.DateTo != null ? session.Date <= searchObject.DateTo : true)
                .Select(session => session.ToDTO())
                .ToListAsync();
            return Ok(searchResults);
        }

        [HttpGet("summary")]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<SessionSummary>> GetSessionSummary([FromQuery] string? username, [FromServices] ISummaryService summaryService)
        {
            if (username is null)
            {
                return BadRequest();
            }

            SessionSummary summary = await summaryService.GetSessionSummary(username, _context);
            return Ok(summary);
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
