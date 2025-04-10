using Eventflow.Domain.Models.Models;
using Eventflow.Domain.Models.ViewModels;

namespace Eventflow.Application.Services.Interfaces
{
    public interface IPersonalEventService
    {
        public Task CreateAsync(PersonalEvent personalEvent);
        public Task<List<PersonalEventWithCategoryNameViewModel>> GetEventsWithCategoryNamesAsync(int userId, int year, int month);
        public Task<PersonalEvent?> GetByIdAsync(int id);
        public Task UpdatePersonalEventAsync(PersonalEvent personalEvent);
        public Task<List<PersonalEventWithCategoryNameViewModel>> GetAcceptedInvitedEventsAsync(int userId, int year, int month);
    }
}
