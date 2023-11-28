using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PracticeTrackerAPI.Models
{
    /// <summary>
    /// Data transfer object that represents a practice session.
    /// </summary>
    public record SessionDTO
    {
        [StringLength(50)]
        [Required(ErrorMessage = "Please specify what you did.")]
        [DefaultValue("practice math")]
        public string Task { get; set; }

        [Required(ErrorMessage = "Please specify how long the session lasted.")]
        public Duration Duration { get; set; }

        [DataType(DataType.Date)]
        [DefaultValue("2020-06-17")]
        public DateOnly? Date { get; set; }

        [DataType(DataType.Time)]
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
        [Range(minimum: 0, maximum: 23)]
        public int Hours { get; set; }

        [Range(minimum: 0, maximum: 59)]
        public int Minutes { get; set; }
    }
}
