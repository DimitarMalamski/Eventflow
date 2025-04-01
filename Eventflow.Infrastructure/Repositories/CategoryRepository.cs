using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
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
    }
}
