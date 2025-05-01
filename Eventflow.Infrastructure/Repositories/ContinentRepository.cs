using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
using Eventflow.Infrastructure.Data.Interfaces;
using System.Data;

namespace Eventflow.Infrastructure.Repositories
{
    public class ContinentRepository : IContinentRepository
    {
        private readonly IDbHelper _dbHelper;
        public ContinentRepository(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public async Task<List<Continent>> GetAllContinentsAsync()
        {
            string query = "SELECT Id, Name FROM Continent";

            var dt = await _dbHelper.ExecuteQueryAsync(query);

            List<Continent> continents = new List<Continent>();

            foreach (DataRow row in dt.Rows)
            {
                continents.Add(new Continent
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString()!
                });
            }

            return continents;
        }
        public async Task<int> GetOrInsertContinentAsync(string continentName)
        {
            string checkIfContinentExistsQuery = "SELECT Id FROM Continent WHERE Name = @Name";

            var parameters = new Dictionary<string, object>
            {
                { "@Name", continentName }
            };

            object? result = await _dbHelper.ExecuteScalarAsync(checkIfContinentExistsQuery, parameters);

            if (result != null && int.TryParse(result.ToString(), out int existingId))
            {
                return existingId;
            }

            string insertContinentQuery = "INSERT INTO Continent (Name) VALUES (@Name); SELECT SCOPE_IDENTITY();";

            object? newId = await _dbHelper.ExecuteScalarAsync(insertContinentQuery, parameters);

            return Convert.ToInt32(newId);
        }
    }
}
