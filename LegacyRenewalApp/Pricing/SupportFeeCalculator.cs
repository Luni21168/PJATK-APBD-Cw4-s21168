using LegacyRenewalApp.Abstractions;

namespace LegacyRenewalApp.Pricing
{
    public class SupportFeeCalculator : ISupportFeeCalculator
    {
        public decimal Calculate(string normalizedPlanCode, bool includePremiumSupport)
        {
            if (!includePremiumSupport)
            {
                return 0m;
            }

            if (normalizedPlanCode == "START")
            {
                return 250m;
            }

            if (normalizedPlanCode == "PRO")
            {
                return 400m;
            }

            if (normalizedPlanCode == "ENTERPRISE")
            {
                return 700m;
            }

            return 0m;
        }
    }
}