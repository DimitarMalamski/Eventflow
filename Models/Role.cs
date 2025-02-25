using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Eventflow.Common.ValidationConstants.Role;

namespace Eventflow.Models
{
    [Table("Role")]
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(roleNameMaxLength)]
        public string Name { get; set; } = null!;
    }
}
