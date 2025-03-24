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
        public List<Continent> GetAllContinents() => _continentRepository.GetAllContinents();
        public List<Continent> OrderContinentByName()
            => _continentRepository.GetAllContinents()
                    .OrderBy(c => c.Name).ToList();
    }
}
