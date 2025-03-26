using Eventflow.Models.Models;

namespace Eventflow.Repositories.Interfaces
{
    public interface IContinentRepository
    {
        public Task<int> GetOrInsertContinentAsync(string continentName);
        public Task<List<Continent>> GetAllContinentsAsync();
    }
}
