using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Entities;
using Eventflow.Infrastructure.Data.Interfaces;
using System.Data;

namespace Eventflow.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDbHelper _dbHelper;
        public CategoryRepository(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            string getAllCategoriesQuery = "SELECT Id, Name FROM Category";

            var table = await _dbHelper.ExecuteQueryAsync(getAllCategoriesQuery);
            var categories = new List<Category>();

            foreach (DataRow row in table.Rows)
            {
                categories.Add(new Category
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString()!
                });
            }

            return categories;
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            string getCategoryByIdQuery = "SELECT * FROM [Category] WHERE Id = @Id";

            var parameters = new Dictionary<string, object>() {
                { "@Id", categoryId }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(getCategoryByIdQuery, parameters);

            if (dt.Rows.Count == 0) {
                return null;
            }

            var row = dt.Rows[0];

            return new Category {
                Id = Convert.ToInt32(row["Id"]),
                Name = row["Name"].ToString()!
            };
        }
   }
}
