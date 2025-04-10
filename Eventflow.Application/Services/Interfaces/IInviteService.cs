using Eventflow.Domain.Models.Models;

namespace Eventflow.Application.Services.Interfaces
{
    public interface IInviteService
    {
        public Task CreateInviteAsync(Invite invite);
        public Task<List<Invite>> GetAllInvitesByUserIdAsync(int userId);
        public Task UpdateInviteStatusAsync(int inviteId, int statusId);
        public Task<bool> InviteExistsAsync(int eventId, int invitedUserId);
        public Task<List<Invite>> GetInvitesByUserAndStatusAsync(int userId, int statusId);
    }
}
