using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Exceptions;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
using static Eventflow.Domain.Common.CustomErrorMessages.ContinentService;

namespace Eventflow.Application.Services
{
    public class ContinentService : IContinentService
    {
        private readonly IContinentRepository _continentRepository;
        public ContinentService(IContinentRepository continentRepository)
        {
            _continentRepository = continentRepository
                ?? throw new ArgumentNullException(nameof(continentRepository));
        }
        public async Task<List<Continent>> OrderContinentByNameAsync()
        {
            try
            {
                return (await _continentRepository.GetAllContinentsAsync())
                    .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                    .OrderBy(c => c.Name)
                    .ToList();
            }
            catch (Exception)
            {
                throw new ContinentRetrievalException(continentRetrievalFailed);
            }
        }
    }
}
