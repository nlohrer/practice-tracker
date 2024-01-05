using Newtonsoft.Json;
using PracticeTrackerAPI.Models.Session;
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
            { "SecondInitial", Helpers.CreateDTOFromParameters( "learn math", 1, 15, "2020-09-03", "11:30:00") },
            { "ThirdInitial", Helpers.CreateDTOFromParameters("learn violin", 1, 0, "2021-09-04", "16:30:00") },
            { "FourthInitial", Helpers.CreateDTOFromParameters("study", 1, 0, "2021-09-05", "16:30:00") }
        };

        /// <summary>
        /// The JSON strings for the tests that need to send a body.
        /// </summary>
        internal static readonly Dictionary<string, string> RequestBodies = new Dictionary<string, string>
        {
            { "PostNewSuccessful", Helpers.SerializeSessionDTO("learn", 2, 30, "2020-02-15", "06:30:00") },
            { "PostForUser", Helpers.SerializeSessionDTO("be a user", 1, 0, "2023-02-15", "09:15:00") },
            { "PostAndDelete", Helpers.SerializeSessionDTO("learn", 2, 45, "2020-02-15", "06:30:00") },
            { "SearchTasks", """{"task": "violin"}""" },
            { "TaskSearchIgnoresCase", """{"task": "VioLIn"}""" },
            { "SearchByDate", """{"dateFrom": "2020-02-18", "dateTo": "2021-09-04"}""" },
            { "UpdateExisting",  Helpers.SerializeSessionDTO("play cello", 1, 30, "2022-02-15", "12:30:00") },
            { "PostNonValid", """{"hours": 2, "minutes": 0, "date": "2000-03-10"}""" },
            { "GetSessionSummary1", Helpers.SerializeSessionDTO("learn", 2, 20, "2020-02-04", "12:00:00") },
            { "GetSessionSummary2", Helpers.SerializeSessionDTO("learn", 1, 50, "2020-02-04", "12:00:00") },
            { "GetSessionSummary3", Helpers.SerializeSessionDTO("learn", 0, 20, "2021-01-01", "12:00:00") }
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
        public async Task SearchByDate()
        {
            string body = RequestBodies["SearchByDate"];
            HttpContent postContent = Helpers.GetJSONContent(body);
            var postResponse = await _client.PostAsync(SessionSearchUrl, postContent);

            var searchContent = await postResponse.Content.ReadAsStringAsync();
            var foundSessions = JsonConvert.DeserializeObject<List<SessionDTO>>(searchContent);
            Assert.NotNull(foundSessions);

            Assert.Contains(InitialSessions["SecondInitial"], foundSessions);
            Assert.Contains(InitialSessions["ThirdInitial"], foundSessions);

            Assert.DoesNotContain(InitialSessions["FirstInitial"], foundSessions);
            Assert.DoesNotContain(InitialSessions["FourthInitial"], foundSessions);
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
        public async Task PostForUser()
        {
            string username = "user";
            string body = RequestBodies["PostForUser"];
            HttpContent postContent = Helpers.GetJSONContent(body);
            var postResult = await _client.PostAsync($"{SessionUrl}?username={username}", postContent);

            Assert.NotNull(postResult.Headers.Location);

            var getResponse = await _client.GetAsync($"{SessionUrl}?username={username}");
            var content = await getResponse.Content.ReadAsStringAsync();
            var foundSessions = JsonConvert.DeserializeObject<List<SessionDTO>>(content);
            Assert.NotNull(foundSessions);
            Assert.Equal(1, foundSessions.Count());

            await _client.DeleteAsync(postResult.Headers.Location);
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
        public async Task DeleteNotExisting()
        {
            var deleteResponse = await _client.DeleteAsync($"{SessionUrl}/101");
            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
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
        public async Task GetSessionSummary()
        {
            string username = "summary";
            string post1 = RequestBodies["GetSessionSummary1"], post2 = RequestBodies["GetSessionSummary2"], post3 = RequestBodies["GetSessionSummary3"];
            HttpContent postContent1 = Helpers.GetJSONContent(post1), postContent2 = Helpers.GetJSONContent(post2), postContent3 = Helpers.GetJSONContent(post3);
            await _client.PostAsync($"{SessionUrl}?username={username}", postContent1);
            await _client.PostAsync($"{SessionUrl}?username={username}", postContent2);
            await _client.PostAsync($"{SessionUrl}?username={username}", postContent3);

            var response = await _client.GetAsync($"{SessionUrl}/summary?username={username}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.NotEqual("{}", content);

            SessionSummary? receivedSummary = JsonConvert.DeserializeObject<SessionSummary>(content);
            Assert.NotNull(receivedSummary);
            SessionSummary expectedSummary = new SessionSummary {
                Amount = 3,
                DayAmount = 2,
                DurationMean = 90,
                DurationVariance = 2600,
                DurationMinimum = new Duration { Hours = 0, Minutes = 20 },
                DurationMaximum = new Duration { Hours = 2, Minutes = 20 },
                DurationMedian = new Duration { Hours = 1, Minutes = 50 },
                FirstDate = new DateOnly(2020, 2, 4),
                LastDate = new DateOnly(2021, 1, 1)
            };

            Assert.Equal(expectedSummary, receivedSummary);
        }

        [Fact]
        public async Task GetSessionSummaryForNonexistentUser()
        {
            string username = "summaryNotExisting";
            var response = await _client.GetAsync($"{SessionUrl}/summary?username={username}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var getContent = await response.Content.ReadAsStringAsync();
            
            SessionSummary? receivedSummary = JsonConvert.DeserializeObject<SessionSummary>(getContent);
            Assert.NotNull(receivedSummary);
            SessionSummary expectedSummary = new SessionSummary { Amount = 0, DayAmount = 0 };

            Assert.Equal(expectedSummary, receivedSummary);
        }

        [Fact]
        public async Task NeedToProvideNameForSessionSummary()
        {
            var response = await _client.GetAsync($"{SessionUrl}/summary");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}