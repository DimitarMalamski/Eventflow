using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Exceptions;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Entities;
using static Eventflow.Domain.Common.CustomErrorMessages.CategoryService;

namespace Eventflow.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository
                ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            try
            {
                 return (await _categoryRepository.GetAllCategoriesAsync())
                    .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                    .OrderBy(c => c.Name)
                    .ToList();
            }
            catch (Exception)
            {
                throw new CategoryRetrievalException(categoryRetrievalFailed);
            }
        }
    }
}
