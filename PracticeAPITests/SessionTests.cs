using Newtonsoft.Json;
using PracticeTrackerAPI.Models;
using System.Net;

namespace PracticeAPITests
{
    public class SessionTests : IClassFixture<DBFixture>, IClassFixture<SessionWebApplicationFactory<Program>>
    {
        private readonly SessionWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        public static readonly Dictionary<string, string> RequestBodies = new Dictionary<string, string>
        {
            { "FirstInitial", SerializeSession(3, "play violin", "02:30:00", "2020-02-15", "06:30:00") },
            { "SecondInitial", SerializeSession(4, "learn math", "01:15:00", "2021-09-03", "11:30:00") },
            { "Post", SerializeSession(1, "learn", "02:30:00", "2020-02-15", "06:30:00") },
            { "PostAndDelete", SerializeSession(5, "learn", "02:30:00", "2020-02-15", "06:30:00") },
            { "Update",  SerializeSession(4, "play cello", "01:30:00", "2022-02-15", "12:30:00") },
            { "PostNonValid", """{"duration":"02:00:00, "date":"2000-03-10"}""" }
        };

        public SessionTests(SessionWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetFirst()
        {
            var response = await _client.GetAsync("/api/Session/3");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            Session? contentAsSession = JsonConvert.DeserializeObject<Session>(content);
            Session? expectedSession = JsonConvert.DeserializeObject<Session>(RequestBodies["FirstInitial"]);
            Assert.NotNull(contentAsSession);

            Assert.Equal(expectedSession, contentAsSession);
        }

        [Fact]
        public async Task GetNotExisting()
        {
            var response = await _client.GetAsync("/api/Session/5");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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

            Session? receivedSession = JsonConvert.DeserializeObject<Session>(getContent);
            Session? expectedSession = JsonConvert.DeserializeObject<Session>(body);
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

            Session? receivedSession = JsonConvert.DeserializeObject<Session>(getContent);
            Session? expectedSession = JsonConvert.DeserializeObject<Session>(body);
            Assert.Equal(expectedSession, receivedSession);
        }

        [Fact]
        public async Task UpdateNonExisting()
        {
            string body = RequestBodies["Update"];
            HttpContent putContent = getJSONContent(body);
            var putResult = await _client.PutAsync("/api/Session/5", putContent);
            Assert.Equal(HttpStatusCode.BadRequest, putResult.StatusCode);
        }

        [Fact]
        public async Task DeleteExistingSuccessful()
        {
            var deleteResponse = await _client.DeleteAsync("/api/Session/3");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync("/api/Session/3");
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

        public static string SerializeSession(int id, string task, string duration, string date, string time)
        {
            string json = $$"""
                {"id": {{id}}, "task": "{{task}}", "duration": "{{duration}}", "date": "{{date}}", "time": "{{time}}" }
                """;
            return json;
        }
    }
}