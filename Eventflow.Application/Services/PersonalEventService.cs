using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;

namespace Eventflow.Application.Services
{
    public class PersonalEventService : IPersonalEventService
    {
        private readonly IPersonalEventRepository _personalEventRepository;
        public PersonalEventService(IPersonalEventRepository personalEventRepository)
        {
            _personalEventRepository = personalEventRepository;
        }

        public async Task CreateAsync(PersonalEvent personalEvent)
            => await _personalEventRepository.CreateEventAsync(personalEvent);

        public async Task<List<PersonalEvent>> GetEventsByUserAndMonth(int userId, int year, int month)
            => await _personalEventRepository.GetByUserAndMonthAsync(userId, year, month);
    }
}
