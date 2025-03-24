using Eventflow.Models.Models;

namespace Eventflow.Services.Interfaces
{
    public interface IContinentService
    {
        List<Continent> GetAllContinents();

        List<Continent> OrderContinentByName();
    }
}
