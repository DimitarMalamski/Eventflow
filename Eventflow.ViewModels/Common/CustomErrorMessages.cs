namespace Eventflow.ViewModels.Common
{
    public static class CustomErrorMessages
    {
        public static class UserService
        {
            public const string userUsernameCannotBeNull = "Username cannot be null or empty.";
        }

        public static class PersonalEventReminderService
        {
            public const string personalEventReminderCannotBeNull = "PersonalEventReminder cannot be null.";
            public const string personalEventRemindedrTitleCannotBeNull = "Reminder title cannot be empty.";
            public const string personalEventReminderDataCannotBeInThePast = "Reminder date cannot be in the past.";
            public const string personalEventReminderHasNoAccessToCreate = "You are not allowed to create a reminder for this event.";
            public const string pageMustBeGreaterThanZero = "Page number must be greater than or equal to 1.";
            public const string pageSizeMustBeGreaterThanZero = "Page size must be greater than 0.";
            public static string ReminderNotFound(int reminderId) => $"Reminder with ID {reminderId} was not found.";
            public const string personalReminderMarkAsReadNotAllowed = "You are not allowed to mark this reminder as read.";
            public const string personalReminderToggleLikeNotAllowed = "You are not allowed to like or unlike this reminder.";
        }

        public static class CategoryService
        {
            public const string categoryRetrievalFailed = "Failed to retrieve categories from the repository.";
        }
        public static class CountryService
        {
            public const string countryRetrievalFailed = "Failed to retrieve countries from the repository.";
            public const string continentIdWasInvalid = "Continent ID must be greater than zero.";
        }
        public static class StatusService
        {
            public const string statusRetrievalFailed = "Failed to retrieve status options from the repository.";
        }
        public static class ContinentService
        {
            public const string continentRetrievalFailed = "Failed to retrieve continents from the repository.";
        }
        public static class Login
        {
            public const string loginInputCannotBeNull = "Login input or password cannot be empty.";
            public const string loginInputRequired = "Username of email is required.";
            public const string loginPasswordRequired = "Password is required."; 
            public const string loginInputInvalid = $"Username/email should be at least 4 characters long";
            public const string loginPasswordInvalid = "Password must be at least 8 characters, contain an uppercase letter, a number, and a special symbol.";
        } 
        public static class Register
        {
            public const string registerUserAlreadyExists = "Username or email already exists.";

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
        public static class PersonalEvent
        {
            public const string personalEventTitleRequired = "Title is required.";
            public const string personalEventDateRequired = "Date is required.";

            public const string personalEventTitleInvalid = "Title cannot be more than 50 characters long.";
            public const string personalEventDescriptionInvalid = "Description cannot be more than 256 characters long.";
        }
    }
}
