using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticeTrackerAPI.Models.User
{
    /// <summary>
    /// Represents a user.
    /// </summary>
    public record User
    {
        /// <summary>
        /// The id of the user.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int? Id {  get; set; }

        /// <summary>
        /// The name of the user.
        /// </summary>
        [StringLength(20)]
        public string Username {  get; set; }

        /// <summary>
        /// The Primary group the user is assigned to.
        /// </summary>
        [StringLength(20)]
        public string? Group {  get; set; }
    }
}
