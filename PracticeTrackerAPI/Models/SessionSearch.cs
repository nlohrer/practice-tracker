using System.ComponentModel.DataAnnotations;

namespace PracticeTrackerAPI.Models
{
    public record SessionSearch
    {
        [StringLength(50)]
        public string? Task { get; set; }
    }
}
