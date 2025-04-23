using Eventflow.Domain.Enums;
using Eventflow.Domain.Models.Models;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface IInviteRepository
    {
        public Task CreateInviteAsync(Invite invite);
        public Task<List<Invite>> GetAllInvitesByUserIdAsync(int userId);
        public Task UpdateInviteStatusAsync(int inviteId, int statusId);
        public Task<bool> InviteExistsAsync(int eventId, int invitedUserId);
        public Task<List<Invite>> GetInvitesByUserAndStatusAsync(int userId, int statusId);
        public Task<List<Invite>> GetExpiredPendingInvitesAsync();
        public Task<bool> HasPendingInvitesAsync(int userId);
        public Task<bool> HasUserAcceptedInviteAsync(int userId, int personalEventId);
        public Task MarkInviteAsLeftAsync(int userId, int eventId);
        public Task<InviteActionResult> CreateOrResetInviteAsync(Invite invite);
    }
}
