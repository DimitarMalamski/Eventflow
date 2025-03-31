using System.ComponentModel.DataAnnotations;
using static Eventflow.Common.ValidationConstants.Status;

namespace Eventflow.Domain.Models.Models
{
    public class Status
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(statusNameMaxLength)]
        public string Name { get; set; } = null!;
    }
}
