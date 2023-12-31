﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticeTrackerAPI.Models.Session
{
    /// <summary>
    /// Represents a practice session.
    /// </summary>
    public record Session
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int? Id { get; set; }

        [StringLength(20)]
        public string? Username { get; set; }

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

        /// <summary>
        /// Returns a SessionDTO that corresponds to the Session.
        /// </summary>
        /// <returns>A SessionDTO corresponding to the Session.</returns>
        public SessionDTO ToDTO()
        {
            return new SessionDTO
            {
                Task = Task,
                Duration = new Duration { Hours = Duration.Hours, Minutes = Duration.Minutes },
                Date = Date,
                Time = Time
            };
        }

        public SessionResponse ToResponse()
        {
            return new SessionResponse
            {
                Id = Id,
                Task = Task,
                Duration = new Duration { Hours = Duration.Hours, Minutes = Duration.Minutes },
                Date = Date,
                Time = Time
            };
        }

    }
}
