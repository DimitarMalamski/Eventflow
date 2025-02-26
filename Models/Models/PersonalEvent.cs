using System.ComponentModel.DataAnnotations;
using static Eventflow.Common.ValidationConstants.PersonalEvent;

namespace Eventflow.Models.Models
{
    public class PersonalEvent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(personalEventTitleMaxLength)]
        public string Title { get; set; } = null!;

        [StringLength(personalEventDescriptionMaxLength)]
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;

        [Required]
        public DateTime Date { get; set; }
        public int? CategoryId { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
