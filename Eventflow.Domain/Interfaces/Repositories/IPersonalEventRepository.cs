using Eventflow.Domain.Models.Entities;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface IPersonalEventRepository
    {
        public Task CreateEventAsync(PersonalEvent personalEvent);
        public Task<List<PersonalEvent>> GetByUserAndMonthAsync(int userId, int year, int month);
        public Task<PersonalEvent?> GetPersonalEventByIdAsync(int id);
        public Task UpdatePersonalEventAsync(PersonalEvent personalEvent);
        public Task<List<PersonalEvent>> GetAcceptedInvitedEventsAsync(int userId);
        public Task<bool> UserHasAccessToEventAsync(int eventId, int userId);
        public Task<int> GetPersonalEventsCountAsync();
        public Task<List<PersonalEvent>> GetRecentPersonalEventsAsync(int count);
    }
}
