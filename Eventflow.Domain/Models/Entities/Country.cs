using System.ComponentModel.DataAnnotations;
using static Eventflow.Domain.Common.ValidationConstants.Country;

namespace Eventflow.Domain.Models.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(countryNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        public string FlagPath { get; set; } = null!;

        [Required]
        public int ContinentId { get; set; }
    }
}
