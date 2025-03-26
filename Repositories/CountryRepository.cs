using Eventflow.Data;
using Eventflow.Models.DTOs;
using Eventflow.Repositories.Interfaces;
using System.Data;

namespace Eventflow.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DbHelper _dbHelper;
        public CountryRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public bool CountryExists(string countryName)
        {
            string checkIfCountryExistsQuery = "SELECT 1 FROM Country WHERE Name = @Name";
            var parameters = new Dictionary<string, object> 
            { 
                { "@Name", countryName }
            };

            var result = _dbHelper.ExecuteScalar(checkIfCountryExistsQuery, parameters);

            return result != null;
        }

        public List<CountryDto> GetAllCountriesByContinentId(int continentId)
        {
            string query = @"SELECT Id, Name 
                     FROM Country 
                     WHERE ContinentId = @ContinentId";

            var parameters = new Dictionary<string, object>
            {
                { "@ContinentId", continentId }
            };

            var dt = _dbHelper.ExecuteQuery(query, parameters);

            List<CountryDto> countries = new List<CountryDto>();

            foreach (DataRow row in dt.Rows)
            {
                countries.Add(new CountryDto
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString()!,
                    Base64Flag = null              
                });
            }

            return countries;
        }

        public void InsertCountry(string countryName, int continentId)
        {
            string insertCountryQuery = @"INSERT INTO Country (Name, ContinentId)
                                     VALUES (@Name, @ContinentId)";

            var countryParams = new Dictionary<string, object>
            {
                {"@Name", countryName},
                {"@ContinentId", continentId}
            };

            _dbHelper.ExecuteNonQuery(insertCountryQuery, countryParams);
        }
    }
}
