using Eventflow.Domain.Models.Models;

namespace Eventflow.Application.Services.Interfaces
{
    public interface IPersonalEventService
    {
        public Task CreateAsync(PersonalEvent personalEvent);
        public Task<List<PersonalEvent>> GetEventsByUserAndMonth(int userId, int year, int month);
    }
}
