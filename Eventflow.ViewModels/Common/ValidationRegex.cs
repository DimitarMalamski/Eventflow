namespace Eventflow.ViewModels.Common
{
    public static class ValidationRegex
    {
        public const string PasswordPattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?""{}|<>]).{8,}$";
        public const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    }
}
