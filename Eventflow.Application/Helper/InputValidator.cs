using System.Text.RegularExpressions;
using static Eventflow.Domain.Common.ValidationConstants.User;
using static Eventflow.Domain.Common.ValidationRegex;

namespace Eventflow.Application.Helper
{
    public static class InputValidator
    {
        public static bool IsValidUsername(string username)
        {
            return !string.IsNullOrWhiteSpace(username)
                   && username.Length >= userUsernameMinLength
                   && username.Length <= userUsernameMaxLength;
        }
        public static bool IsValidFirstname(string firstname)
        {
            return !string.IsNullOrWhiteSpace(firstname)
                   && firstname.Length >= userFirstnameMinLength
                   && firstname.Length <= userFirstnameMaxLength;
        }
        public static bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password)
                   && password.Length >= userPasswordMinLength
                   && password.Length <= userPasswordMaxLength
                   && Regex.IsMatch(password, PasswordPattern);
        }
        public static bool IsValidEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email)
                   && email.Length <= userEmailMaxLength
                   && Regex.IsMatch(email, EmailPattern);
        }
        public static bool IsValidLoginInput(string loginInput)
        {
            return !string.IsNullOrWhiteSpace(loginInput)
                   && loginInput.Length >= userUsernameMinLength
                   && loginInput.Length <= userEmailMaxLength;
        }
        public static bool IsValidLoginPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password)
                   && password.Length <= userPasswordMaxLength;
        }
    }
}
