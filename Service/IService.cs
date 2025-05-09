using DBEntities;

namespace Service1;

public interface IService
{
    public IEnumerable<Currency> SearchByCountry(string CountryName);
    public IEnumerable<string> SearchByCurrency(string CurrencyName);
    bool CreateCurrency(CurrencyRequestDTO request);
    bool UpdateCurrency(CurrencyRequestDTO request);
    bool CountryExists(string countryName);
    bool CurrencyExists(string currencyName);
}