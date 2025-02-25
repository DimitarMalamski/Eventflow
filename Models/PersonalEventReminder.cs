using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Eventflow.Common.ValidationConstants.PersonalEventReminder;

namespace Eventflow.Models
{
    [Table("PersonalEventReminder")]
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

        [ForeignKey("PersonalEventId")]
        public PersonalEvent PersonalEvent { get; set; } = null!;
    }
}
