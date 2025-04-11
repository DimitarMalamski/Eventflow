using Eventflow.Domain.Models.Models;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface IPersonalEventRepository
    {
        public Task CreateEventAsync(PersonalEvent personalEvent);
        public Task<List<PersonalEvent>> GetByUserAndMonthAsync(int userId, int year, int month);
        public Task<PersonalEvent?> GetPersonalEventByIdAsync(int id);
        public Task UpdatePersonalEventAsync(PersonalEvent personalEvent);
        public Task<List<PersonalEvent>> GetAcceptedInvitedEventsAsync(int userId);
        public Task<List<PersonalEvent>> GetAllPersonalEventsByUserIdAsync(int userId);
    }
}
