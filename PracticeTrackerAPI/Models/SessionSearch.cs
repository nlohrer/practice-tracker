using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PracticeTrackerAPI.Models
{
    /// <summary>
    /// Represents the parameters for searching a session.
    /// </summary>
    public record SessionSearch
    {
        /// <summary>
        /// A string that the session task should contain.
        /// </summary>
        [StringLength(50)]
        [DefaultValue("learn")]
        public string? Task { get; set; }
    }
}
