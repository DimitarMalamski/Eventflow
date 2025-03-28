namespace Eventflow.Common
{
    public static class CustomErrorMessages
    {
        public static class Login
        {
            public const string loginInputRequired = "Username of email is required.";
            public const string loginPasswordRequired = "Password is required."; 

            public const string loginPasswordRegex = @"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?""{}|<>]).{8,}$";

            public const string loginInputInvalid = $"Username/email should be at least 4 characters long";
            public const string loginPasswordInvalid = "Password must be at least 8 characters, contain an uppercase letter, a number, and a special symbol.";
        } 
        public static class Register
        {
            public const string registerUsernameRequired = "Username is required.";
            public const string registerEmailRequired = "Email is required.";
            public const string registerPasswordRequired = "Password is required.";
            public const string registerConfirmPasswordRequired = "Confirm password is required.";
            public const string registerFirstnameRequired = "Firstname is required.";

            public const string registerUsernameInvalid = "Username should be between 4 and 50 characters long.";
            public const string registerEmailInvalid = "Please enter a valid email address.";
            public const string registerFirstnameInvalid = "Firstname should be between 4 and 50 characters long.";
            public const string registerPasswordInvalid = "Password must be at least 8 characters, contain an uppercase letter, a number, and a special symbol.";
            public const string registerPasswordNoMatch = "Password does not match.";
            public const string registerLastnameInvalid = "Lastname must not be over 50 characters.";
        }
    }
}
