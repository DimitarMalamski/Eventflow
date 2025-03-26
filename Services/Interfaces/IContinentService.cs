using Eventflow.Models.Models;

namespace Eventflow.Services.Interfaces
{
    public interface IContinentService
    {
        public Task<List<Continent>> GetAllContinentsAsync();

        public Task<List<Continent>> OrderContinentByNameAsync();
    }
}
