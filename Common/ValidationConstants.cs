namespace Eventflow.Common
{
    public static class ValidationConstants
    {
        public static class User
        {
            public const int userUsernameMaxLength = 50;
            public const int userFirstnameMaxLength = 50;
            public const int userLastnameMaxLength = 50;
            public const int userEmailMaxLength = 256;
            public const int userPasswordHashMaxLength = 256;
        }
        public static class Role
        {
            public const int roleNameMaxLength = 20;
        }
        public static class Category
        {
            public const int categoryNameMaxLength = 50;
        }
        public static class PersonalEvent
        {
            public const int personalEventTitleMaxLength = 50;
            public const int personalEventDescriptionMaxLength = 256;
        }
        public static class PersonalEventReminder
        {
            public const int PersonalEventReminderTitleMaxLength = 50;
            public const int PersonalEventReminderDescriptionMaxLength = 256;
        }
    }
}
