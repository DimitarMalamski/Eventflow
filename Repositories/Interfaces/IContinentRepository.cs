using Eventflow.Models.Models;

namespace Eventflow.Repositories.Interfaces
{
    public interface IContinentRepository
    {
        List<Continent> GetAllContinents();
    }
}
