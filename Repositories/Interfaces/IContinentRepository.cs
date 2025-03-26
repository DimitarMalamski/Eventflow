using Eventflow.Models.Models;

namespace Eventflow.Repositories.Interfaces
{
    public interface IContinentRepository
    {
        int GetOrInsertContinent(string continentName);
        public List<Continent> GetAllContinents();
    }
}
