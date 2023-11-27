using Newtonsoft.Json;
using PracticeTrackerAPI.Models;
using System.Net;

namespace PracticeAPITests
{
    public class SessionTests : IClassFixture<DBFixture>, IClassFixture<SessionWebApplicationFactory<Program>>
    {
        private readonly SessionWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        public static readonly Dictionary<string, SessionDTO> InitialSessions = new Dictionary<string, SessionDTO>
        {
            { "FirstInitial", JsonConvert.DeserializeObject<SessionDTO>(SerializeSessionDTO("play violin", 2, 30, "2020-02-15", "06:30:00")) },
            { "SecondInitial", JsonConvert.DeserializeObject<SessionDTO>(SerializeSessionDTO( "learn math", 1, 15, "2021-09-03", "11:30:00")) },
            { "ThirdInitial", JsonConvert.DeserializeObject<SessionDTO>(SerializeSessionDTO("learn violin", 1, 0, "2021-09-05", "16:30:00")) },
            { "FourthInitial", JsonConvert.DeserializeObject<SessionDTO>(SerializeSessionDTO("study", 1, 0, "2021-09-05", "16:30:00")) }
        };
        public static readonly Dictionary<string, string> RequestBodies = new Dictionary<string, string>
        {
            { "Post", SerializeSessionDTO("learn", 2, 30, "2020-02-15", "06:30:00") },
            { "PostAndDelete", SerializeSessionDTO("learn", 2, 45, "2020-02-15", "06:30:00") },
            { "SearchTasks", """{"task": "violin"}""" },
            { "Update",  SerializeSessionDTO("play cello", 1, 30, "2022-02-15", "12:30:00") },
            { "PostNonValid", """{"hours": 2, "minutes": 0, "date": "2000-03-10"}""" }
        };

        public SessionTests(SessionWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetFirst()
        {
            var response = await _client.GetAsync("/api/Session/1");
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
            var response = await _client.GetAsync("/api/Session/100");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SearchTasks()
        {
            string body = RequestBodies["SearchTasks"];
            HttpContent postContent = getJSONContent(body);
            var postResponse = await _client.PostAsync("/api/Session/search", postContent);

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
            string body = RequestBodies["Post"];
            HttpContent postContent = getJSONContent(body);
            var postResult = await _client.PostAsync("/api/Session", postContent);
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
            HttpContent postContent = getJSONContent(body);
            var postResult = await _client.PostAsync("/api/Session", postContent);
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
            HttpContent postContent = getJSONContent(body);
            var postResult = await _client.PostAsync("/api/Session", postContent);
            Assert.Equal(HttpStatusCode.BadRequest, postResult.StatusCode);
        }

        [Fact]
        public async Task UpdateExisting()
        {
            string body = RequestBodies["Update"];
            HttpContent putContent = getJSONContent(body);
            var putResult = await _client.PutAsync("/api/Session/4", putContent);
            Assert.Equal(HttpStatusCode.NoContent, putResult.StatusCode);

            var getResult = await _client.GetAsync("/api/Session/4");
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
            string body = RequestBodies["Update"];
            HttpContent putContent = getJSONContent(body);
            var putResult = await _client.PutAsync("/api/Session/100", putContent);
            Assert.Equal(HttpStatusCode.NotFound, putResult.StatusCode);
        }

        [Fact]
        public async Task DeleteExistingSuccessful()
        {
            var deleteResponse = await _client.DeleteAsync("/api/Session/2");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync("/api/Session/2");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteNotExisting()
        {
            var deleteResponse = await _client.DeleteAsync("/api/Session/5");
            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
        }

        public HttpContent getJSONContent(string body)
        {
            StringContent content = new StringContent(body);
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");
            return content;
        }

        public static string SerializeSessionDTO(string task, int hours, int minutes, string date, string time)
        {
            string json = $$"""
                {"task": "{{task}}", "duration": {"hours": {{hours}}, "minutes": {{minutes}}}, "date": "{{date}}", "time": "{{time}}" }
                """;
            return json;
        }
    }
}