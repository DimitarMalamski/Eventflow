using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Enums;
using Eventflow.Domain.Helper;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Entities;
using Eventflow.DTOs.DTOs;
using static Eventflow.Application.Helper.StringMatchHelper;
using static Eventflow.Application.Mapper.ViewModelMapper.InviteMapper;
using static Eventflow.Domain.Common.CustomErrorMessages.InviteService;

namespace Eventflow.Application.Services
{
    public class InviteService : IInviteService
    {
        private readonly IInviteRepository _inviteRepository;
        private readonly IUserRepository _userRepository;
        public InviteService(
            IInviteRepository inviteRepository,
            IUserRepository userRepository)
        {
            _inviteRepository = inviteRepository;
            _userRepository = userRepository;
        }
        public async Task AutoDeclineExpiredInvitesAsync()
        {
            var expiredInvites = await _inviteRepository.GetExpiredPendingInvitesAsync();

            foreach(var invite in expiredInvites)
            {
                await _inviteRepository.UpdateInviteStatusAsync(invite.Id, 3);
            }
        }
        public async Task<int> CountPendingInvitesAsync(int userId)
            => (await _inviteRepository.GetInvitesByUserAndStatusAsync(userId, 1)).Count;
        public async Task<InviteActionResult> CreateOrResetInviteAsync(Invite invite)
        {
            var invitedUser = await _userRepository.GetUserByIdAsync(invite.InvitedUserId);

            if (invitedUser == null || invitedUser.IsDeleted) {
                throw new InvalidOperationException("Cannot invite a deleted or non-existent user.");
            }

            return await _inviteRepository.CreateOrResetInviteAsync(invite);
        }
        public async Task<List<Invite>> GetAllInvitesByUserIdAsync(int userId)
            => await _inviteRepository.GetAllInvitesByUserIdAsync(userId);
        public async Task<List<Invite>> GetInvitesByUserAndStatusAsync(int userId, int statusId)
            => await _inviteRepository.GetInvitesByUserAndStatusAsync(userId, statusId);
        public async Task<PaginatedInvitesDto> GetPaginatedFilteredInvitesAsync(
            int userId, 
            int statusId,
            string? search,
            string? sortBy,
            int page,
            int pageSize)
        {
            var invites = await _inviteRepository.GetInvitesByUserAndStatusAsync(userId, statusId);

            invites = invites
                .Where(i => i.StatusId != InviteStatusHelper.KickedOut)
                .ToList();

            var inviteDto = ToInviteDtoList(invites);

            return PaginateAndWrap(inviteDto, search, sortBy, page, pageSize);

        }
        public async Task<bool> HasPendingInvitesAsync(int userId)
                => await _inviteRepository.HasPendingInvitesAsync(userId);
        public async Task<bool> HasUserAcceptedInviteAsync(int userId, int personalEventId)
            => await _inviteRepository.HasUserAcceptedInviteAsync(userId, personalEventId);
        public async Task<bool> InviteExistsAsync(int eventId, int invitedUserId)
            => await _inviteRepository.InviteExistsAsync(eventId, invitedUserId);
        public async Task LeaveEventAsync(int userId, int eventId)
            => await _inviteRepository.MarkInviteAsLeftAsync(userId, eventId);
        public async Task UpdateInviteStatusAsync(int inviteId, int statusId)
            => await _inviteRepository.UpdateInviteStatusAsync(inviteId, statusId);
        private List<InviteDto> FilterInvites(List<InviteDto> invites, string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return invites;
            }

            return invites
                .Where(r =>
                    Match(r.EventTitle, search) ||
                    Match(r.EventDescription, search) ||
                    Match(r.InvitedByUsername, search))
                .ToList();
        }
        private List<InviteDto> SortInvites(List<InviteDto> invites, string? sortBy)
        {
            return sortBy?.ToLower() switch
            {
                "event" => invites.OrderBy(i => i.EventTitle).ToList(),
                "date" => invites.OrderBy(i => i.EventDate).ToList(),
                "inviter" => invites.OrderBy(i => i.InvitedByUsername).ToList(),
                _ => invites.OrderBy(r => r.Id).ToList()
            };
        }
        private PaginatedInvitesDto PaginateAndWrap(IEnumerable<InviteDto> invites,
            string? search, 
            string? sortBy, 
            int page, 
            int pageSize)
        {
            if (page < 1)
            {
                throw new ArgumentException(pageMustBeGreaterThanZero, nameof(page));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentException(pageSizeMustBeGreaterThanZero, nameof(pageSize));
            }

            var InvitesList = invites.ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                InvitesList = FilterInvites(InvitesList, search);
            }

            var sorted = SortInvites(InvitesList, sortBy);

            var totalCount = sorted.Count;
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));

            var paginated = sorted
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedInvitesDto
            {
                Invites = paginated,
                TotalPages = totalPages,
                CurrentPage = page
            };
        }

        public async Task<InviteAdminDto?> GetInviteAsync(int eventId, int invitedUserId)
        {
            var invite = await _inviteRepository.GetInviteByEventAndUserAsync(eventId, invitedUserId);

            if (invite == null) {
                return null;
            }

            var user = await _userRepository.GetUserByIdAsync(invite.InvitedUserId);

            return new InviteAdminDto {
                Id = invite.Id,
                EventId = invite.PersonalEventId,
                InvitedUserId = invite.InvitedUserId,
                StatusId = invite.StatusId,
                Status = Enum.GetName(typeof(InviteStatus), invite.StatusId) ?? "Unknown",
            };
        }
        public async Task DeleteInviteAsync(int eventId, int invitedUserId)
            => await _inviteRepository.SoftDeleteInviteAsync(eventId, invitedUserId);
   }
}
