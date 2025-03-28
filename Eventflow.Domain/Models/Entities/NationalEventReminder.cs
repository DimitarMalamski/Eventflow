using System.ComponentModel.DataAnnotations;
using static Eventflow.Common.ValidationConstants.NationalEventReminder;

namespace Eventflow.Models.Models
{
    public class NationalEventReminder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(nationalEventReminderTitleMaxLength)]
        public string Title { get; set; } = null!;

        [StringLength(nationalEventReminderDesriptionMaxLength)]
        public string? Description { get; set; }

        [Required]
        public DateTime Date { get; set; }
        public bool IsRead { get; set; } = false;

        [Required]
        public int NationalEventId { get; set; }
    }
}
