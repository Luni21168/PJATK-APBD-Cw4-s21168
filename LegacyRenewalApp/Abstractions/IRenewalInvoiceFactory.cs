using LegacyRenewalApp.Pricing;

namespace LegacyRenewalApp.Abstractions
{
    public interface IRenewalInvoiceFactory
    {
        RenewalInvoice Create(
            Customer customer,
            string normalizedPlanCode,
            string normalizedPaymentMethod,
            int customerId,
            int seatCount,
            PricingResult pricingResult);
    }
}