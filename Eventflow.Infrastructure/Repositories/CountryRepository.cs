using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Entities;
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
            string query = @"SELECT Id, Name, FlagUrl, ContinentId, ISOCode 
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
                    ContinentId = Convert.ToInt32(row["ContinentId"]),
                    ISOCode = row["ISOCode"].ToString()!
                });
            }

            return countries;
        }
        public async Task<Country?> GetCountryByIdAsync(int countryId)
        {
            string getCountryByIdQuery = @"
                    SELECT Id, Name, FlagUrl, ContinentId, ISOCode
                    FROM Country
                    WHERE Id = @CountryId";

            var parameters = new Dictionary<string, object>()
            {
                { "@CountryId", countryId }
            };

            var dt = await _dbHelper.ExecuteQueryAsync(getCountryByIdQuery, parameters);

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            var row = dt.Rows[0];

            return new Country
            {
                Id = Convert.ToInt32(row["Id"]),
                Name = row["Name"].ToString()!,
                FlagUrl = row["FlagUrl"].ToString()!,
                ContinentId = Convert.ToInt32(row["ContinentId"]),
                ISOCode = row["ISOCode"].ToString()!
            };
        }
        public async Task<Country> GetCountryByISOCodeAsync(string isoCode)
        {
            try
            {
                string query = "SELECT * FROM Country WHERE ISOCode = @ISOCode";

                var parameters = new Dictionary<string, object>
                {
                    { "@ISOCode", isoCode }
                };

                var dt = await _dbHelper.ExecuteQueryAsync(query, parameters);



                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];

                    return new Country
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Name = row["Name"].ToString()!,
                        ISOCode = row["ISOCode"].ToString()!,
                        FlagUrl = row["FlagUrl"].ToString()!,
                        ContinentId = Convert.ToInt32(row["ContinentId"])
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error fetching country with ISOCode {isoCode}: {ex.Message}");
                return null;
            }
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
        public async Task InsertHolidayAsync(int countryId, string name, string date, string description)
        {
            // Ensure the fields are not null or empty before inserting
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(date))
            {
                Console.Error.WriteLine($"Skipping holiday: {name} due to missing data.");
                return;
            }

            string query = @"INSERT INTO NationalEvent (CountryId, Title, Date, Description) 
                     VALUES (@CountryId, @Title, @Date, @Description)";

            var parameters = new Dictionary<string, object>
            {
                { "@CountryId", countryId },
                { "@Title", name },
                { "@Date", date },
                { "@Description", description }
            };

            await _dbHelper.ExecuteNonQueryAsync(query, parameters);
            Console.WriteLine($"Inserted holiday: {name} for country: {countryId}");
        }
    }
}
