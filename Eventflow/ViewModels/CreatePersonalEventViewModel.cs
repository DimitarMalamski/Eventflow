using Eventflow.Domain.Models.Entities;
using System.ComponentModel.DataAnnotations;
using static Eventflow.Domain.Common.CustomErrorMessages.PersonalEvent;
using static Eventflow.Domain.Common.ValidationConstants.PersonalEvent;

namespace Eventflow.ViewModels
{
    public class CreatePersonalEventViewModel
    {
        public int? Id { get; set; }

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
        public List<Category>? Categories { get; set; }
    }
}
