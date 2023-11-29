using Newtonsoft.Json;
using PracticeTrackerAPI.Models;
using System.Net;

namespace PracticeAPITests
{
    public class SessionTests : IClassFixture<DBFixture>, IClassFixture<SessionWebApplicationFactory<Program>>
    {
        private readonly SessionWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        /// <summary>
        /// The initial objects in the test database.
        /// </summary>
        internal static readonly Dictionary<string, SessionDTO> InitialSessions = new Dictionary<string, SessionDTO>
        {
            { "FirstInitial", Helpers.CreateDTOFromParameters("play violin", 2, 30, "2020-02-15", "06:30:00") },
            { "SecondInitial", Helpers.CreateDTOFromParameters( "learn math", 1, 15, "2021-09-03", "11:30:00") },
            { "ThirdInitial", Helpers.CreateDTOFromParameters("learn violin", 1, 0, "2021-09-05", "16:30:00") },
            { "FourthInitial", Helpers.CreateDTOFromParameters("study", 1, 0, "2021-09-05", "16:30:00") }
        };

        /// <summary>
        /// The JSON strings for the tests that need to send a body.
        /// </summary>
        internal static readonly Dictionary<string, string> RequestBodies = new Dictionary<string, string>
        {
            { "PostNewSuccessful", Helpers.SerializeSessionDTO("learn", 2, 30, "2020-02-15", "06:30:00") },
            { "PostAndDelete", Helpers.SerializeSessionDTO("learn", 2, 45, "2020-02-15", "06:30:00") },
            { "SearchTasks", """{"task": "violin"}""" },
            { "TaskSearchIgnoresCase", """{"task": "VioLIn"}""" },
            { "UpdateExisting",  Helpers.SerializeSessionDTO("play cello", 1, 30, "2022-02-15", "12:30:00") },
            { "PostNonValid", """{"hours": 2, "minutes": 0, "date": "2000-03-10"}""" }
        };

        internal static readonly string SessionUrl = "/api/Session";
        internal static readonly string SessionSearchUrl = "/api/Session/Search";

        public SessionTests(SessionWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetFirst()
        {
            var response = await _client.GetAsync($"{SessionUrl}/1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            SessionDTO? contentAsSession = JsonConvert.DeserializeObject<SessionDTO>(content);
            SessionDTO? expectedSession = InitialSessions["FirstInitial"];
            Assert.NotNull(contentAsSession);

            Assert.Equal(expectedSession, contentAsSession);
        }

        [Fact]
        public async Task GetNotExisting()
        {
            var response = await _client.GetAsync($"{SessionUrl}/100");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SearchTasks()
        {
            string body = RequestBodies["SearchTasks"];
            HttpContent postContent = Helpers.GetJSONContent(body);
            var postResponse = await _client.PostAsync(SessionSearchUrl, postContent);

            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var searchContent = await postResponse.Content.ReadAsStringAsync();
            var foundSessions = JsonConvert.DeserializeObject<List<SessionDTO>>(searchContent);
            Assert.NotNull(foundSessions);

            SessionDTO? firstExpected = InitialSessions["FirstInitial"];
            SessionDTO? secondExpected = InitialSessions["ThirdInitial"];

            Assert.Contains<SessionDTO>(firstExpected, foundSessions);
            Assert.Contains<SessionDTO>(secondExpected, foundSessions);
        }

        [Fact]
        public async Task TaskSearchIgnoresCase()
        {
            string body = RequestBodies["TaskSearchIgnoresCase"];
            HttpContent postContent = Helpers.GetJSONContent(body);
            var postResponse = await _client.PostAsync(SessionSearchUrl, postContent);

            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var searchContent = await postResponse.Content.ReadAsStringAsync();
            var foundSessions = JsonConvert.DeserializeObject<List<SessionDTO>>(searchContent);
            Assert.NotNull(foundSessions);

            SessionDTO? firstExpected = InitialSessions["FirstInitial"];
            SessionDTO? secondExpected = InitialSessions["ThirdInitial"];

            Assert.Contains<SessionDTO>(firstExpected, foundSessions);
            Assert.Contains<SessionDTO>(secondExpected, foundSessions);
        }

        [Fact]
        public async Task PostNewSuccessful()
        {
            string body = RequestBodies["PostNewSuccessful"];
            HttpContent postContent = Helpers.GetJSONContent(body);
            var postResult = await _client.PostAsync(SessionUrl, postContent);
            Assert.Equal(HttpStatusCode.Created, postResult.StatusCode);

            Uri? newUri = postResult.Headers.Location;
            Assert.NotNull(newUri);

            var getResult = await _client.GetAsync(newUri);
            Assert.Equal(HttpStatusCode.OK, getResult.StatusCode);

            var getContent = await getResult.Content.ReadAsStringAsync();
            Assert.NotNull(getContent);

            SessionDTO? receivedSession = JsonConvert.DeserializeObject<SessionDTO>(getContent);
            SessionDTO? expectedSession = JsonConvert.DeserializeObject<SessionDTO>(body);
            Assert.Equal(expectedSession, receivedSession);
        }

        [Fact]
        public async Task PostAndDelete()
        {
            string body = RequestBodies["PostAndDelete"];
            HttpContent postContent = Helpers.GetJSONContent(body);
            var postResult = await _client.PostAsync(SessionUrl, postContent);
            Assert.Equal(HttpStatusCode.Created, postResult.StatusCode);

            Uri? newUri = postResult.Headers.Location;
            Assert.NotNull(newUri);

            var deleteResult = await _client.DeleteAsync(newUri);
            Assert.Equal(HttpStatusCode.NoContent, deleteResult.StatusCode);

            var getResult = await _client.DeleteAsync(newUri);
            Assert.Equal(HttpStatusCode.NotFound, getResult.StatusCode);
        }

        [Fact]
        public async Task PostNonValid()
        {
            string body = RequestBodies["PostNonValid"];
            HttpContent postContent = Helpers.GetJSONContent(body);
            var postResult = await _client.PostAsync(SessionUrl, postContent);
            Assert.Equal(HttpStatusCode.BadRequest, postResult.StatusCode);
        }

        [Fact]
        public async Task UpdateExisting()
        {
            string body = RequestBodies["UpdateExisting"];
            HttpContent putContent = Helpers.GetJSONContent(body);
            var putResult = await _client.PutAsync($"{SessionUrl}/4", putContent);
            Assert.Equal(HttpStatusCode.NoContent, putResult.StatusCode);

            var getResult = await _client.GetAsync($"{SessionUrl}/4");
            Assert.Equal(HttpStatusCode.OK, getResult.StatusCode);

            var getContent = await getResult.Content.ReadAsStringAsync();
            Assert.NotNull(getContent);

            SessionDTO? receivedSession = JsonConvert.DeserializeObject<SessionDTO>(getContent);
            SessionDTO? expectedSession = JsonConvert.DeserializeObject<SessionDTO>(body);
            Assert.Equal(expectedSession, receivedSession);
        }

        [Fact]
        public async Task UpdateNonExisting()
        {
            string body = RequestBodies["UpdateExisting"];  // May be any arbitrary valid body
            HttpContent putContent = Helpers.GetJSONContent(body);
            var putResult = await _client.PutAsync($"{SessionUrl}/100", putContent);
            Assert.Equal(HttpStatusCode.NotFound, putResult.StatusCode);
        }

        [Fact]
        public async Task DeleteExistingSuccessful()
        {
            var deleteResponse = await _client.DeleteAsync($"{SessionUrl}/2");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"{SessionUrl}/2");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteNotExisting()
        {
            var deleteResponse = await _client.DeleteAsync($"{SessionUrl}/5");
            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
        }

    }

    public class Helpers
    {
        /// <summary>
        /// Turns the JSON string into an HttpContent object with the fitting 'content-type' header.
        /// </summary>
        /// <param name="body">The JSON string for the body.</param>
        /// <returns>An HttpContent the JSON string as its body and with 'content-type' set to 'application/json'</returns>
        public static HttpContent GetJSONContent(string body)
        {
            StringContent content = new StringContent(body);
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");
            return content;
        }

        /// <summary>
        /// Serializes the given parameters into a JSON SessionDTO.
        /// </summary>
        /// <returns>A JSON string representation of the SessionDTO.</returns>
        public static string SerializeSessionDTO(string task, int hours, int minutes, string date, string time)
        {
            string json = $$"""
                {"task": "{{task}}", "duration": {"hours": {{hours}}, "minutes": {{minutes}}}, "date": "{{date}}", "time": "{{time}}" }
                """;
            return json;
        }

        /// <summary>
        /// Creates a SessionDTO from the given parameters.
        /// </summary>
        /// <returns>A SessionDTO with the provided values.</returns>
        public static SessionDTO CreateDTOFromParameters(string task, int hours, int minutes, string date, string time)
        {
            return new SessionDTO
            {
                Task = task,
                Duration = new Duration
                {
                    Hours = hours,
                    Minutes = minutes,
                },
                Date = DateOnly.Parse(date),
                Time = TimeOnly.Parse(time)
            };
        }

    }
}