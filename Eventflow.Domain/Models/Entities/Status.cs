using System.ComponentModel.DataAnnotations;
using static Eventflow.Domain.Common.ValidationConstants.Status;

namespace Eventflow.Domain.Models.Entities
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
