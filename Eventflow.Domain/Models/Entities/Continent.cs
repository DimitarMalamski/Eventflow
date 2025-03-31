using System.ComponentModel.DataAnnotations;
using static Eventflow.Common.ValidationConstants.Continent;

namespace Eventflow.Domain.Models.Models
{
    public class Continent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(continentNameMaxLength)]
        public string Name { get; set; } = null!;
    }
}
