using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PracticeTrackerAPI.Models
{
    /// <summary>
    /// A practice session.
    /// </summary>
    public record SessionDTO
    {
        /// <summary>
        /// The task you did in the practice session.
        /// </summary>
        [StringLength(50)]
        [Required(ErrorMessage = "Please specify what you did.")]
        [DefaultValue("learn math")]
        public string Task { get; set; }

        /// <summary>
        /// How long you practiced for.
        /// </summary>
        [Required(ErrorMessage = "Please specify how long the session lasted.")]
        public Duration Duration { get; set; }

        /// <summary>
        /// The date of the session.
        /// </summary>
        [DataType(DataType.Date)]
        [DefaultValue("2020-06-17")]
        public DateOnly? Date { get; set; }

        /// <summary>
        /// The time the session started at.
        /// </summary>
        [DataType(DataType.Time)]
        [DefaultValue("12:30:15")]
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
        [DefaultValue(1)]
        public int Hours { get; set; }

        [Range(minimum: 0, maximum: 59)]
        [DefaultValue(30)]
        public int Minutes { get; set; }
    }
}
