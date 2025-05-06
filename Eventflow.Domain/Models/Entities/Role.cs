using System.ComponentModel.DataAnnotations;
using static Eventflow.Domain.Common.ValidationConstants.Role;

namespace Eventflow.Domain.Models.Entities
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
