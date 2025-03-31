using System.ComponentModel.DataAnnotations;
using static Eventflow.Common.ValidationConstants.Role;

namespace Eventflow.Domain.Models.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(roleNameMaxLength)]
        public string Name { get; set; } = null!;
    }
}
