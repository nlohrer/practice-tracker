namespace PracticeTrackerAPI.Models
{
    /// <summary>
    /// Data transfer object that represents a practice session.
    /// </summary>
    public record SessionDTO
    {
        public string Task { get; set; }
        public Duration Duration { get; set; }
        public DateOnly? Date { get; set; }
        public TimeOnly? Time { get; set; }

        public Session ToSession()
        {
            return new Session
            {
                Task = this.Task,
                Duration = new TimeSpan(this.Duration.Hours, this.Duration.Minutes, 0),
                Date = this.Date,
                Time = this.Time
            };
        }
    }

    /// <summary>
    /// Represents a duration in hours and minutes.
    /// </summary>
    public record Duration
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }
    }
}
