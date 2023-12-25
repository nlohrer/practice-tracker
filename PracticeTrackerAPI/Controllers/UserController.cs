using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeTrackerAPI.Models;
using PracticeTrackerAPI.Models.User;

namespace PracticeTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly SessionContext _context;

        public UserController(SessionContext context)
        {
            _context = context;
        }

        /// <summary>
        /// List all users.
        /// </summary>
        /// <returns>A list of all users.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/User
        /// </remarks>
        /// <response code="200">Returns all users.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Get a specific user by <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The <paramref name="id"/> of the user.</param>
        /// <returns>The user with the specified <paramref name="id"/>.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/User/1
        /// </remarks>
        /// <response code="200">Returns the requested user.</response>
        /// <response code="404">If no user matched the given <paramref name="id"/>.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            User? user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="user">A JSON object representing the user.</param>
        /// <returns>The newly created user.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/User
        ///     {
        ///         "username": "user1",
        ///         "group": "group3"
        ///     }
        /// </remarks>
        /// <response code="201">Returns the newly created user.</response>
        /// <response code="400">If the user could not be validated.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            if (ModelState.IsValid)
            {
                return CreatedAtAction("GetUser", new { id = user.Id }, user);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Deletes a specific user by id.
        /// </summary>
        /// <param name="id">The <paramref name="id"/> of the user.</param>
        /// <returns>Nothing.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /api/User/1
        /// </remarks>
        /// <response code="204">If the user was deleted successfully.</response>
        /// <response code="404">If no user matched the given <paramref name="id"/>.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
