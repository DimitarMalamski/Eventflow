using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Enums;
using Eventflow.Domain.Interfaces.Repositories;

namespace Eventflow.Application.Services
{
    public class MessageNotificationService : IMessageNotificationService
    {
        private readonly IPersonalEventReminderRepository _personalEventReminderRepository;
        private readonly IInviteRepository _inviteRepository;
        public MessageNotificationService(IPersonalEventReminderRepository personalEventReminderRepository,
            IInviteRepository inviteRepository)
        {
            _inviteRepository = inviteRepository;
            _personalEventReminderRepository = personalEventReminderRepository;
        }
        public async Task<bool> HasPendingInvitesAsync(int userId)
        {
            var invites = await _inviteRepository.GetInvitesByUserAndStatusAsync(userId, 1);
            return invites.Any();          
        }
        public async Task<bool> HasTodaysUnreadRemindersAsync(int userId)
        {
            var reminders = await _personalEventReminderRepository.GetRemindersWithEventAndTitleByUserIdAsync(userId);
            return reminders.Any(r => r.Status == ReminderStatus.Unread 
                                && r.Date.Date == DateTime.Today);
        }
    }
}
