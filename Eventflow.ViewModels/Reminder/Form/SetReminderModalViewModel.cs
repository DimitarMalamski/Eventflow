using System.ComponentModel.DataAnnotations;
using static Eventflow.ViewModels.Common.ValidationConstants.PersonalEventReminder;

namespace Eventflow.ViewModels.Reminder.Form
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
