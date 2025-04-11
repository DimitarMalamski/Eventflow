using System.ComponentModel.DataAnnotations;
using static Eventflow.Domain.Common.ValidationConstants.PersonalEventReminder;

namespace Eventflow.Domain.Models.ViewModels
{
    public class SetReminderModalViewModel
    {
        public int PersonalEventId { get; set; }

        [Required]
        [StringLength(PersonalEventReminderTitleMaxLength)]
        public string Title { get; set; } = null!;

        [StringLength(PersonalEventReminderDescriptionMaxLength)]
        public string? Description { get; set; }

        [Required]
        public DateTime ReminderDate { get; set; }


    }
}
