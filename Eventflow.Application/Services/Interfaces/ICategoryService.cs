using Eventflow.Domain.Models.Entities;

namespace Eventflow.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        public Task<List<Category>> GetAllCategoriesAsync();
    }
}
