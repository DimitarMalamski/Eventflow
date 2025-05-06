namespace Eventflow.ViewModels.Common
{
    public static class ValidationConstants
    {
        public static class User
        {
            public const int userUsernameMaxLength = 50;
            public const int userUsernameMinLength = 4;
            public const int userFirstnameMaxLength = 50;
            public const int userFirstnameMinLength = 4;
            public const int userLastnameMaxLength = 50;
            public const int userEmailMaxLength = 256;
            public const int userPasswordMaxLength = 50;
            public const int userPasswordMinLength = 8;
            public const int userPasswordHashMaxLength = 255;
            public const int userPasswordSaltMaxLength = 255;
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
        public static class Status
        {
            public const int statusNameMaxLength = 50;
        }
        public static class Continent
        {
            public const int continentNameMaxLength = 50;
        }
        public static class Country
        {
            public const int countryNameMaxLength = 50;
        }
        public static class NationalEvent
        {
            public const int nationalEventTitleMaxLength = 50;
            public const int nationalEventDescriptionMaxLength = 256;
        }
        public static class NationalEventReminder
        {
            public const int nationalEventReminderTitleMaxLength = 255;
            public const int nationalEventReminderDesriptionMaxLength = Int32.MaxValue;
        }
    }
}
