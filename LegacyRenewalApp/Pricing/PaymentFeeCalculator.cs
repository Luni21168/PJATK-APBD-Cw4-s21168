using System;
using LegacyRenewalApp.Abstractions;

namespace LegacyRenewalApp.Pricing
{
    public class PaymentFeeCalculator : IPaymentFeeCalculator
    {
        public decimal Calculate(string normalizedPaymentMethod, decimal subtotalAfterDiscount, decimal supportFee)
        {
            decimal feeBase = subtotalAfterDiscount + supportFee;

            if (normalizedPaymentMethod == "CARD")
            {
                return feeBase * 0.02m;
            }

            if (normalizedPaymentMethod == "BANK_TRANSFER")
            {
                return feeBase * 0.01m;
            }

            if (normalizedPaymentMethod == "PAYPAL")
            {
                return feeBase * 0.035m;
            }

            if (normalizedPaymentMethod == "INVOICE")
            {
                return 0m;
            }

            throw new ArgumentException("Unsupported payment method");
        }
    }
}