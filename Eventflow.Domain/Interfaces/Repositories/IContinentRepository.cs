using Eventflow.Domain.Models.Models;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface IContinentRepository
    {
        public Task<int> GetOrInsertContinentAsync(string continentName);
        public Task<List<Continent>> GetAllContinentsAsync();
    }
}
