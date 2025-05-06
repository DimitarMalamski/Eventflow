namespace Eventflow.Utilities
{
    public static class EnumMapper
    {
        public static ViewModels.Reminder.Enums.ReminderStatusEnum ToViewModelStatus(DTOs.Enums.ReminderStatusEnum dtoStatus)
            => (ViewModels.Reminder.Enums.ReminderStatusEnum)(int)dtoStatus;

        public static ViewModels.Reminder.Enums.ReminderStatusEnum ToViewModelStatus(Domain.Enums.ReminderStatus domainStatus)
            => (ViewModels.Reminder.Enums.ReminderStatusEnum)(int)domainStatus;
    }
}
