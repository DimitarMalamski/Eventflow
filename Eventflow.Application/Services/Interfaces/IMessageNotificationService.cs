namespace Eventflow.Application.Services.Interfaces
{
    public interface IMessageNotificationService
    {
        public Task<bool> HasTodaysUnreadRemindersAsync(int userId);
        public Task<bool> HasPendingInvitesAsync(int userId);
    }
}
