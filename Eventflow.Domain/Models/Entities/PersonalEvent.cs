using System.ComponentModel.DataAnnotations;
using static Eventflow.Domain.Common.ValidationConstants.PersonalEvent;

namespace Eventflow.Domain.Models.Entities
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

        // Navigation property
        public User? User { get; set; }
    }
}
