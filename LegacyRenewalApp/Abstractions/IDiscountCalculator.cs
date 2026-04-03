namespace LegacyRenewalApp.Abstractions
{
    public interface IDiscountCalculator
    {
        Pricing.DiscountResult Calculate(
            Customer customer,
            SubscriptionPlan plan,
            decimal baseAmount,
            int seatCount,
            bool useLoyaltyPoints);
    }
}