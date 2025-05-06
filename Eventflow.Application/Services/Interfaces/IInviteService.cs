using Eventflow.Domain.Enums;
using Eventflow.Domain.Models.Entities;

namespace Eventflow.Application.Services.Interfaces
{
    public interface IInviteService
    {
        public Task<InviteActionResult> CreateOrResetInviteAsync(Invite invite);
        public Task<List<Invite>> GetAllInvitesByUserIdAsync(int userId);
        public Task UpdateInviteStatusAsync(int inviteId, int statusId);
        public Task<bool> InviteExistsAsync(int eventId, int invitedUserId);
        public Task<List<Invite>> GetInvitesByUserAndStatusAsync(int userId, int statusId);
        public Task AutoDeclineExpiredInvitesAsync();
        public Task<bool> HasPendingInvitesAsync(int userId);
        public Task<bool> HasUserAcceptedInviteAsync(int userId, int personalEventId);
        public Task<int> CountPendingInvitesAsync(int userId);
        public Task LeaveEventAsync(int userId, int eventId);
    }
}
