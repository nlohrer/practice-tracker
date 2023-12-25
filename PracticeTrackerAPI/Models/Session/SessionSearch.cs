using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PracticeTrackerAPI.Models.Session
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

        /// <summary>
        /// The minimum date of the session.
        /// </summary>
        [DataType(DataType.Date)]
        public DateOnly? DateFrom { get; set; }

        /// <summary>
        /// The maximum date of the session.
        /// </summary>
        [DataType(DataType.Date)]
        public DateOnly? DateTo { get; set; }
    }
}
