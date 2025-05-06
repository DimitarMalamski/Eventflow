using Eventflow.Domain.Models.Entities;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface IContinentRepository
    {
        public Task<int> GetOrInsertContinentAsync(string continentName);
        public Task<List<Continent>> GetAllContinentsAsync();
    }
}
