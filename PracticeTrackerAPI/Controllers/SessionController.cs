using Microsoft.AspNetCore.Mvc;

namespace PracticeTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            return Ok("Hello world");
        }
    }
}