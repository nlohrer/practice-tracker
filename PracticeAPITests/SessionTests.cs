using Newtonsoft.Json;
using PracticeTrackerAPI.Models;
using System.Net;

namespace PracticeAPITests
{
    public class SessionTests : IClassFixture<DBFixture>, IClassFixture<SessionWebApplicationFactory<Program>>
    {
        private readonly SessionWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

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
            Assert.True(response.IsSuccessStatusCode);
            string content = await response.Content.ReadAsStringAsync();
            Session? contentAsSession = JsonConvert.DeserializeObject<Session>(content);
            Assert.NotNull(contentAsSession);

            Session expectedSession = new Session(
                                id: 3, task: "play violin",
                                duration: TimeSpan.Parse("02:30:00"),
                                date: DateOnly.Parse("2020/02/15"),
                                time: TimeOnly.Parse("06:30")
                                );
            Assert.Equal(expectedSession, contentAsSession);
        }

        [Fact]
        public async Task PostNewSuccessful()
        {
            string body = """
                {"id": 1, "task":"learn", "duration":"02:30:00", "date":"2020-02-15", "time":"06:30:00"}
            """;
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
        public async Task UpdateExisting()
        {
            string body = """
                {"id": 4, "task":"play cello", "duration":"01:30:00", "date":"2022-02-15", "time":"12:30:00"}
            """;
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
        public async Task DeleteExistingSuccessful()
        {
            var deleteResponse = await _client.DeleteAsync("/api/Session/3");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync("/api/Session/3");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        public HttpContent getJSONContent(string body)
        {
            StringContent content = new StringContent(body);
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");
            return content;
        }
    }
}