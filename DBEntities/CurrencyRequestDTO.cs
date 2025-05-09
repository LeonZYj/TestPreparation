namespace DBEntities;

public class CurrencyRequestDTO
{
    public string CurrencyName { get; set; }
    public float RatetoUSD { get; set; }
    public List<Country> Countries { get; set; }
}