using Eventflow.Domain.Models.Models;
using System.ComponentModel.DataAnnotations;
using static Eventflow.Domain.Common.ValidationConstants.PersonalEvent;
using static Eventflow.Domain.Common.CustomErrorMessages.PersonalEvent;

namespace Eventflow.Domain.Models.ViewModels
{
    public class CreatePersonalEventViewModel
    {
        [Required(ErrorMessage = personalEventTitleRequired)]
        [StringLength(personalEventTitleMaxLength,
            ErrorMessage = personalEventTitleInvalid)]
        public string Title { get; set; } = null!;

        [StringLength(personalEventDescriptionMaxLength,
            ErrorMessage = personalEventDescriptionInvalid)]
        public string? Description { get; set; }

        [Required(ErrorMessage = personalEventDateRequired)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public int? CategoryId { get; set; }
        public List<Category>? Category { get; set; }
    }
}
