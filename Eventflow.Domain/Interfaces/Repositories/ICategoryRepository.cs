using Eventflow.Domain.Models.Models;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        public Task<List<Category>> GetAllCategoriesAsync();
    }
}
