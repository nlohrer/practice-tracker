using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticeTrackerAPI.Models
{
    public record Session
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Please specify what you did")]
        public string Task { get; set; }
        [DataType(DataType.Time)]
        [Required(ErrorMessage = "Please specify how long the session lasted")]
        public TimeSpan Duration { get; set; }
        [DataType(DataType.DateTime)]
        public DateOnly? Date { get; set; }
        [DataType(DataType.Time)]
        public TimeOnly? Time { get; set; }

        public Session(int id, string task, TimeSpan duration, DateOnly? date, TimeOnly? time)
        {
            Id = id;
            Task = task;
            Duration = duration;
            Date = date;
            Time = time;
        }

    }
}
