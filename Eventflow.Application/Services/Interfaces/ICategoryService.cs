using Eventflow.Domain.Models.Models;

namespace Eventflow.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        public Task<List<Category>> GetAllCategoriesAsync();
    }
}
