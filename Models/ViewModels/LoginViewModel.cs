using System.ComponentModel.DataAnnotations;
using static Eventflow.Common.CustomErrorMessages.Login;
using static Eventflow.Common.ValidationConstants.User;

namespace Eventflow.Models.ViewModels
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
