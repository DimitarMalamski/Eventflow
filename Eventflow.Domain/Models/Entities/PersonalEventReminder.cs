using Eventflow.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using static Eventflow.Domain.Common.ValidationConstants.PersonalEventReminder;

namespace Eventflow.Domain.Models.Models
{
    public class PersonalEventReminder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(PersonalEventReminderTitleMaxLength)]
        public string Title { get; set; } = null!;

        [StringLength(PersonalEventReminderDescriptionMaxLength)]
        public string? Description { get; set; }

        [Required]
        public DateTime Date { get; set; }
        public DateTime? ReadAt { get; set; }
        public ReminderStatus Status { get; set; }

        [Required]
        public int PersonalEventId { get; set; }
        public bool IsLiked { get; set; } = false;

        // Navigation property
        public PersonalEvent? PersonalEvent { get; set; }
    }
}
