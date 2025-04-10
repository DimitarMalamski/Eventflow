using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;

namespace Eventflow.Application.Services
{
    public class InviteService : IInviteService
    {
        private readonly IInviteRepository _inviteRepository;
        public InviteService(IInviteRepository inviteRepository)
        {
            _inviteRepository = inviteRepository;
        }

        public async Task CreateInviteAsync(Invite invite)
            => await _inviteRepository.CreateInviteAsync(invite);

        public async Task<List<Invite>> GetAllInvitesByUserIdAsync(int userId)
            => await _inviteRepository.GetAllInvitesByUserIdAsync(userId);

        public async Task<bool> InviteExistsAsync(int eventId, int invitedUserId)
            => await _inviteRepository.InviteExistsAsync(eventId, invitedUserId);

        public async Task UpdateInviteStatusAsync(int inviteId, int statusId)
            => await _inviteRepository.UpdateInviteStatusAsync(inviteId, statusId);
    }
}
