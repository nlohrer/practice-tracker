using PracticeAPITests;
using PracticeTrackerAPI.Models.User;
using System.Net;

namespace PracticeTrackerAPITests
{
    public class UserTests : IClassFixture<DBFixture>, IClassFixture<SessionWebApplicationFactory<Program>>
    {
        private readonly SessionWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        /// <summary>
        /// The initial objects in the test database.
        /// </summary>
        internal static readonly Dictionary<string, User> InitialUsers = new()
        {
            { "FirstInitial", new User { Username = "user1", Group = "group1" } },
            { "SecondInitial", new User { Username = "user2" } }
        };
        internal static readonly string UserUrl = "/api/User";

        /// <summary>
        /// The JSON strings for the tests that need to send a body.
        /// </summary>
        internal static readonly Dictionary<string, string> RequestBodies = new Dictionary<string, string>
        {
            { "PostNewSuccessful", $$"""
                {"username": "user3"}
                """ }
        };

        public UserTests(SessionWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll()
        {
            var response = await _client.GetAsync(UserUrl);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            Assert.Contains("user1", content);
            Assert.Contains("user2", content);
        }

        [Fact]
        public async Task GetFirst()
        {
            var response = await _client.GetAsync($"{UserUrl}/1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            Assert.Contains("user1", content);
        }

        [Fact]
        public async Task PostNewSuccessful()
        {
            string body = RequestBodies["PostNewSuccessful"];
            HttpContent postContent = Helpers.GetJSONContent(body);
            var postResult = await _client.PostAsync(UserUrl, postContent);

            Assert.Equal(HttpStatusCode.Created, postResult.StatusCode);
            Assert.NotNull(postResult.Headers.Location);

            var getResponse = await _client.GetAsync(postResult.Headers.Location);
            string content = await getResponse.Content.ReadAsStringAsync();
            Assert.Contains("user3", content);
        }

        [Fact]
        public async Task DeleteExistingSuccessful()
        {
            var deleteResponse = await _client.DeleteAsync($"{UserUrl}/2");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync(UserUrl);
            string content = await getResponse.Content.ReadAsStringAsync();
            Assert.DoesNotContain("user2", content);
        }

    }
}
