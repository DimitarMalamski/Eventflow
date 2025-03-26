using Eventflow.Models.Models;
using Eventflow.Repositories.Interfaces;
using Eventflow.Services.Interfaces;

namespace Eventflow.Services
{
    public class ContinentService : IContinentService
    {
        private readonly IContinentRepository _continentRepository;
        public ContinentService(IContinentRepository continentRepository)
        {
            _continentRepository = continentRepository;
        }
        public async Task<List<Continent>> GetAllContinentsAsync() => await _continentRepository.GetAllContinentsAsync();
        public async Task<List<Continent>> OrderContinentByNameAsync()
            => (await _continentRepository.GetAllContinentsAsync()
                ).OrderBy(c => c.Name).ToList();
    }
}
