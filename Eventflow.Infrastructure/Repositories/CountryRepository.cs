using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
using Eventflow.Infrastructure.Data.Interfaces;
using System.Data;

namespace Eventflow.Infrastructure.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly IDbHelper _dbHelper;
        public CountryRepository(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public async Task<bool> CountryExistsAsync(string countryName)
        {
            string checkIfCountryExistsQuery = "SELECT 1 FROM Country WHERE Name = @Name";
            var parameters = new Dictionary<string, object>
            {
                { "@Name", countryName }
            };

            var result = await _dbHelper.ExecuteScalarAsync(checkIfCountryExistsQuery, parameters);

            return result != null;
        }
        public async Task<List<Country>> GetAllCountriesByContinentIdAsync(int continentId)
        {
            string query = @"SELECT Id, Name, FlagUrl, ContinentId 
                     FROM Country 
                     WHERE ContinentId = @ContinentId";

            var parameters = new Dictionary<string, object>
            {
                { "@ContinentId", continentId }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(query, parameters);

            List<Country> countries = new List<Country>();

            foreach (DataRow row in dt.Rows)
            {
                countries.Add(new Country
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString()!,
                    FlagUrl = null!,
                    ContinentId = Convert.ToInt32(row["ContinentId"])
                });
            }

            return countries;
        }
        public async Task InsertCountryAsync(string countryName, int continentId, string flagUrl, string isoCode)
        {
            string insertCountryQuery = @"INSERT INTO Country (Name, ContinentId, FlagUrl, ISOCode)
                                     VALUES (@Name, @ContinentId, @FlagUrl, @ISOCode)";

            var countryParams = new Dictionary<string, object>
            {
                { "@Name", countryName },
                { "@ContinentId", continentId },
                { "@FlagUrl", flagUrl },
                { "@ISOCode", isoCode }
            };

            await _dbHelper.ExecuteNonQueryAsync(insertCountryQuery, countryParams);
        }
    }
}
