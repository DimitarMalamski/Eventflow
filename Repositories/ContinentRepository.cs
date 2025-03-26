using Eventflow.Data;
using Eventflow.Models.Models;
using Eventflow.Repositories.Interfaces;
using System.Data;

namespace Eventflow.Repositories
{
    public class ContinentRepository : IContinentRepository
    {
        private readonly DbHelper _dbHelper;
        public ContinentRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<Continent> GetAllContinents()
        {
            string query = "SELECT Id, Name FROM Continent";
            var dt = _dbHelper.ExecuteQuery(query);

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

        public int GetOrInsertContinent(string continentName)
        {
            string checkIfContinentExistsQuery = "SELECT Id FROM Continent WHERE Name = @Name";

            var parameters = new Dictionary<string, object> 
            { 
                { "@Name", continentName } 
            };

            object result = _dbHelper.ExecuteScalar(checkIfContinentExistsQuery, parameters);

            if (result != null && int.TryParse(result.ToString(), out int existingId))
            {
                return existingId;
            }            

            string insertContinentQuery = "INSERT INTO Continent (Name) VALUES (@Name); SELECT SCOPE_IDENTITY();";

            object newId = _dbHelper.ExecuteScalar(insertContinentQuery, parameters);

            return Convert.ToInt32(newId);
        }
    }
}
