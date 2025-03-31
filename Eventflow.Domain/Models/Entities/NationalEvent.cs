using System.ComponentModel.DataAnnotations;
using static Eventflow.Common.ValidationConstants.NationalEvent;

namespace Eventflow.Domain.Models.Models
{
    public class NationalEvent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(nationalEventTitleMaxLength)]
        public string Title { get; set; } = null!;

        [StringLength(nationalEventDescriptionMaxLength)]
        public string? Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int CountryId { get; set; }
    }
}
