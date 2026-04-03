namespace LegacyRenewalApp.Abstractions
{
    public interface ITaxCalculator
    {
        decimal GetTaxRate(string country);
    }
}