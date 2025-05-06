using System.ComponentModel.DataAnnotations;
using static Eventflow.Domain.Common.ValidationConstants.User;

namespace Eventflow.Domain.Models.Entities
{
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
        [StringLength(userPasswordSaltMaxLength)]
        public string Salt { get; set; } = null!;

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
    }
}
