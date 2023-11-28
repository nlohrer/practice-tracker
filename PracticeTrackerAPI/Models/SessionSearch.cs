using System.ComponentModel.DataAnnotations;

namespace PracticeTrackerAPI.Models
{
    /// <summary>
    /// Represents the parameters for searching a session.
    /// </summary>
    public record SessionSearch
    {
        [StringLength(50)]
        public string? Task { get; set; }
    }
}
