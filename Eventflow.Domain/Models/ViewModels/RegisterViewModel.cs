using System.ComponentModel.DataAnnotations;
using static Eventflow.Common.ValidationConstants.User;
using static Eventflow.Common.CustomErrorMessages.Register;
using static Eventflow.Common.CustomErrorMessages.Login;

namespace Eventflow.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = registerUsernameRequired)]
        [StringLength(userUsernameMaxLength,
            MinimumLength = userUsernameMinLength,
            ErrorMessage = registerUsernameInvalid)]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = registerEmailRequired)]
        [EmailAddress(ErrorMessage = registerEmailInvalid)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = registerFirstnameRequired)]
        [StringLength(userFirstnameMaxLength,
            MinimumLength = userFirstnameMinLength,
            ErrorMessage = registerFirstnameInvalid)]
        public string Firstname { get; set; } = null!;

        [StringLength(userLastnameMaxLength,
            ErrorMessage = registerLastnameInvalid)]
        public string? Lastname { get; set; }

        [Required(ErrorMessage = registerPasswordRequired)]
        [DataType(DataType.Password)]
        [StringLength(userPasswordMaxLength,
            MinimumLength = userPasswordMinLength,
            ErrorMessage = registerPasswordInvalid)]
        [RegularExpression(loginPasswordRegex,
            ErrorMessage = registerPasswordInvalid)]

        public string Password { get; set; } = null!;

        [Required(ErrorMessage = registerConfirmPasswordRequired)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = registerPasswordNoMatch)]
        public string ConfirmPassword { get; set; } = null!;
    }
}
