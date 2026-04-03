namespace LegacyRenewalApp.Abstractions
{
    public interface IPaymentFeeCalculator
    {
        decimal Calculate(string normalizedPaymentMethod, decimal subtotalAfterDiscount, decimal supportFee);
    }
}