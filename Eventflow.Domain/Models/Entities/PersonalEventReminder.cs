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
        public bool IsRead { get; set; } = false;

        [Required]
        public int PersonalEventId { get; set; }
    }
}
