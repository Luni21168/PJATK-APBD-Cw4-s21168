namespace LegacyRenewalApp.Abstractions
{
    public interface ISupportFeeCalculator
    {
        decimal Calculate(string normalizedPlanCode, bool includePremiumSupport);
    }
}