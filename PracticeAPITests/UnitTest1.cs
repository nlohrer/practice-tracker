using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PracticeTracker.Controllers;

namespace PracticeAPITests
{
    public class SessionTests
    {
        [Fact]
        public async void HelloWorldTest()
        {
            SessionController controller = new SessionController();
            ActionResult<string> task = await controller.Get();
            Assert.IsType<OkObjectResult>(task.Result);
            OkObjectResult result = (OkObjectResult) task.Result;

            Assert.Equal("Hello world", result.Value);
        }
    }
}