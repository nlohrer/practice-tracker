using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticeTrackerAPI.Models.Session
{
    /// <summary>
    /// A practice session.
    /// </summary>
    public record SessionResponse
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int? Id { get; set; }

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
                Id = Id,
                Task = Task,
                Duration = new TimeSpan(Duration.Hours, Duration.Minutes, 0),
                Date = Date,
                Time = Time
            };
        }
    }
}
