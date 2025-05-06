using System.ComponentModel.DataAnnotations;
using static Eventflow.ViewModels.Common.CustomErrorMessages.Register;
using static Eventflow.ViewModels.Common.ValidationConstants.User;
using static Eventflow.ViewModels.Common.ValidationRegex;

namespace Eventflow.ViewModels.Account.Form
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
        [RegularExpression(PasswordPattern,
            ErrorMessage = registerPasswordInvalid)]

        public string Password { get; set; } = null!;

        [Required(ErrorMessage = registerConfirmPasswordRequired)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = registerPasswordNoMatch)]
        public string ConfirmPassword { get; set; } = null!;
    }
}
