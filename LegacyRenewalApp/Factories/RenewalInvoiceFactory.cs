using System;
using LegacyRenewalApp.Abstractions;
using LegacyRenewalApp.Pricing;

namespace LegacyRenewalApp.Factories
{
    public class RenewalInvoiceFactory : IRenewalInvoiceFactory
    {
        public RenewalInvoice Create(
            Customer customer,
            string normalizedPlanCode,
            string normalizedPaymentMethod,
            int customerId,
            int seatCount,
            PricingResult pricingResult)
        {
            return new RenewalInvoice
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customerId}-{normalizedPlanCode}",
                CustomerName = customer.FullName,
                PlanCode = normalizedPlanCode,
                PaymentMethod = normalizedPaymentMethod,
                SeatCount = seatCount,
                BaseAmount = Math.Round(pricingResult.BaseAmount, 2, MidpointRounding.AwayFromZero),
                DiscountAmount = Math.Round(pricingResult.DiscountAmount, 2, MidpointRounding.AwayFromZero),
                SupportFee = Math.Round(pricingResult.SupportFee, 2, MidpointRounding.AwayFromZero),
                PaymentFee = Math.Round(pricingResult.PaymentFee, 2, MidpointRounding.AwayFromZero),
                TaxAmount = Math.Round(pricingResult.TaxAmount, 2, MidpointRounding.AwayFromZero),
                FinalAmount = Math.Round(pricingResult.FinalAmount, 2, MidpointRounding.AwayFromZero),
                Notes = pricingResult.Notes.Trim(),
                GeneratedAt = DateTime.UtcNow
            };
        }
    }
}