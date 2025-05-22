using Eventflow.Domain.Models.Entities;
using Eventflow.DTOs.DTOs;

namespace Eventflow.Application.Services.Interfaces
{
    public interface IPersonalEventService
    {
        public Task CreateAsync(PersonalEvent personalEvent);
        public Task<List<PersonalEventWithCategoryNameDto>> GetEventsWithCategoryNamesAsync(int userId, int year, int month);
        public Task<PersonalEvent?> GetPersonalEventByIdAsync(int id);
        public Task UpdatePersonalEventAsync(PersonalEvent personalEvent);
        public Task<List<PersonalEventWithCategoryNameDto>> GetAcceptedInvitedEventsAsync(int userId, int year, int month);
        public Task<int> GetPersonalEventsCountAsync();
        public Task<List<RecentPersonalEventDto>> GetRecentPersonalEventsAsync(int count);
        public Task<List<ManageEventDto>> GetAllManageEventsAsync();
        public Task<ManageEventDto?> UpdateEventFromAdminAsync(EditEventDto dto);
        public Task<List<EventParticipantDto>> GetParticipantsByEventIdAsync(int eventId);
        public Task<bool> SoftDeleteEventAsync(int eventId, int userId);
        public Task<List<PersonalEventWithCategoryNameDto>> GetGlobalEventsWithCategoryAsync(int year, int month);
    }
}
