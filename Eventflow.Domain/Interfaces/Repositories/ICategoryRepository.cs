using Eventflow.Domain.Models.Entities;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        public Task<List<Category>> GetAllCategoriesAsync();
        public Task<Category?> GetCategoryByIdAsync(int categoryId);
    }
}
