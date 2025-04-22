using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;

namespace Eventflow.Application.Services
{
    public class ContinentService : IContinentService
    {
        private readonly IContinentRepository _continentRepository;
        public ContinentService(IContinentRepository continentRepository)
        {
            _continentRepository = continentRepository;
        }
        public async Task<List<Continent>> OrderContinentByNameAsync()
            => (await _continentRepository.GetAllContinentsAsync()
                ).OrderBy(c => c.Name).ToList();
    }
}
