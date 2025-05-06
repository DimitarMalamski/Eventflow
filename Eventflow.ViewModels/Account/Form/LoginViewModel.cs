using System.ComponentModel.DataAnnotations;
using static Eventflow.ViewModels.Common.CustomErrorMessages.Login;
using static Eventflow.ViewModels.Common.ValidationConstants.User;

namespace Eventflow.ViewModels.Account.Form
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = loginInputRequired)]
        [StringLength(userEmailMaxLength,
            MinimumLength = userUsernameMinLength,
            ErrorMessage = loginInputInvalid)]
        [Display(Name = "Username / Email")]
        public string LoginInput { get; set; } = null!;

        [Required(ErrorMessage = loginPasswordRequired)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
