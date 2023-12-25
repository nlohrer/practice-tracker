using PracticeTrackerAPI.Models.Session;

namespace PracticeAPITests
{
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