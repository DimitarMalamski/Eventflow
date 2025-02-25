using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Eventflow.Common.ValidationConstants.User;

namespace Eventflow.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(userUsernameMaxLength)]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(userPasswordHashMaxLength)]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [StringLength(userFirstnameMaxLength)]
        public string Firstname { get; set; } = null!;

        [StringLength(userLastnameMaxLength)]
        public string? Lastname { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(userEmailMaxLength)]
        public string Email { get; set; } = null!;

        [Required]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; } = null!;
    }
}
