using Eventflow.Domain.Models.DTOs;
using Eventflow.Domain.Models.Entities;

namespace Eventflow.Application.Services.Interfaces
{
    public interface IPersonalEventService
    {
        public Task CreateAsync(PersonalEvent personalEvent);
        public Task<List<PersonalEventWithCategoryNameDto>> GetEventsWithCategoryNamesAsync(int userId, int year, int month);
        public Task<PersonalEvent?> GetPersonalEventByIdAsync(int id);
        public Task UpdatePersonalEventAsync(PersonalEvent personalEvent);
        public Task<List<PersonalEventWithCategoryNameDto>> GetAcceptedInvitedEventsAsync(int userId, int year, int month);
    }
}
