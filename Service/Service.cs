using DBEntities;
using Microsoft.Data.SqlClient;

namespace Service1;

public class Service : IService
{
    private readonly string _connectionString;

    public Service(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<Currency> SearchByCountry(string CountryName)
    {
        var currencies = new List<Currency>();

        var query = @"SELECT C.Name AS CURRENCYNAME, C.Rate AS CURRENCYRATE FROM CURRENCY C
                    JOIN CurrencyCountry CC ON C.ID = CC.CURRENCY_ID 
                    JOIN COUNTRY CO ON CC.COUNTRY_ID = CO.ID
                    WHERE CO.NAME = @CountryName";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CountryName", CountryName);
            var reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        currencies.Add(new Currency
                        {
                            Name = reader["CURRENCYNAME"].ToString(),
                            Rate = (float)Convert.ToDouble(reader["CURRENCYRATE"])
                        });
                    }
                }
            }
            finally
            {
                reader.Close();
            }
        }
        return currencies;
    }

    public IEnumerable<string> SearchByCurrency(string CurrencyName)
    {
        var countries = new List<string>();

        var query = @"SELECT CO.NAME AS CountryName FROM Country CO
                    JOIN CurrencyCountry CC ON CO.ID = CC.Country.ID
                    JOIN Currency C ON CC.CurrencyID = C.ID
                    WHERE C.NAME = @CurrencyName";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CurrencyName", CurrencyName);
            
            var reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        countries.Add(reader["CountryName"].ToString());
                    }
                }
            }
            finally
            {
                reader.Close();
            }
        }
        return countries;
    }

    public bool CreateCurrency(CurrencyRequestDTO request)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            foreach (var country in request.Countries)
            {
                if (!CountryExists(country.Name))
                    return false;
            }

            var query = "INSERT INTO CURRENCY(NAME,RATE) VALUES (@CurrencyName, @CurrencyRate)";

            var insertCommand = new SqlCommand(query, connection);
            insertCommand.Parameters.AddWithValue("@CurrencyName", request.CurrencyName);
            insertCommand.Parameters.AddWithValue("@CurrencyRate", request.RatetoUSD);
            connection.Open();
            insertCommand.ExecuteNonQuery();

            var currencyIdQuery = "SELECT ID FROM CURRENCY WHERE NAME = @CurrencyName";
            var currencyIdCommand = new SqlCommand(currencyIdQuery, connection);
            currencyIdCommand.Parameters.AddWithValue("@CurrencyName", request.CurrencyName);
            var currencyId = (int)currencyIdCommand.ExecuteScalar();

            foreach (var country in request.Countries)
            {
                var insertquery =
                    "INSERT INTO CurrencyCountry(Currency_ID, Country_ID) VALUES (@CurrencyId, SELECT ID FROM COUNTRY WHERE NAME = @CountryName)";
                var insertCommand2 = new SqlCommand(insertquery, connection);
                insertCommand2.Parameters.AddWithValue("@CurrencyId", currencyId);
                insertCommand2.Parameters.AddWithValue("@CountryName", country.Name);
            }
        }
        return true;
    }

    public bool UpdateCurrency(CurrencyRequestDTO request)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            foreach (var country in request.Countries)
            {
                if (!CountryExists(country.Name))
                    return false;
            }

            var query = "UPDATE CURRECY SET NAME = @CurrencyName, RATE = @CurrencyRate";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CurrencyName", request.CurrencyName);
            command.Parameters.AddWithValue("@CurrencyRate", request.RatetoUSD);
            connection.Open();
            command.ExecuteNonQuery();
            
            
            var deleteQuery = "DEELTE FROM CurrencyCountry WHERE Currency_ID = (SELECT ID FROM CURRENCY WHERE NAME = @CurrencyName)";
            var deleteCommand = new SqlCommand(deleteQuery, connection);
            deleteCommand.Parameters.AddWithValue("@CurrencyName", request.CurrencyName);
            deleteCommand.ExecuteNonQuery();


            foreach (var country in request.Countries)
            {
                var insertQuery = "INSERT INTO CurrencyCountry(Currency_ID, Country_ID) SELECT (SELECT ID FROM CURRENCY WHERE NAME - @CurrencyName),(SELECT ID FROM Country WHERE NAME = @CountryNAme)";
                var insertCommand = new SqlCommand(insertQuery, connection);
                insertCommand.Parameters.AddWithValue("@CurrencyName", request.CurrencyName);
                insertCommand.Parameters.AddWithValue("@CountryName", country.Name);
                insertCommand.ExecuteNonQuery();
            }
            return true;
        }
    }

    public bool CountryExists(string countryName)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var query = "SELECT COUNT(1) FROM COUNTRY WHERE name = @Countryname";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Countryname", countryName);

            connection.Open();
            var result = (int)command.ExecuteScalar();
            return result > 0;
        }
    }


    public bool CurrencyExists(string currencyName)
    {
        using (var connection = new SqlConnection(_connectionString))
        {

            var query = "SELECT COUNT(1) FROM CURRENCY WHERE name= @Currencyname";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Currencyname", currencyName);
            
            connection.Open();
            var result = (int)command.ExecuteScalar();
            return result > 0;
        }
    }
}