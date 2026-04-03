using System;
using LegacyRenewalApp.Abstractions;
using LegacyRenewalApp.Billing;
using LegacyRenewalApp.Factories;
using LegacyRenewalApp.Pricing;
using LegacyRenewalApp.Validation;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly IRenewalRequestValidator _validator;
        private readonly IBillingGateway _billingGateway;
        private readonly IDiscountCalculator _discountCalculator;
        private readonly ISupportFeeCalculator _supportFeeCalculator;
        private readonly IPaymentFeeCalculator _paymentFeeCalculator;
        private readonly ITaxCalculator _taxCalculator;
        private readonly IRenewalInvoiceFactory _invoiceFactory;

        public SubscriptionRenewalService()
            : this(
                new CustomerRepository(),
                new SubscriptionPlanRepository(),
                new RenewalRequestValidator(),
                new LegacyBillingGatewayAdapter(),
                new DiscountCalculator(),
                new SupportFeeCalculator(),
                new PaymentFeeCalculator(),
                new TaxCalculator(),
                new RenewalInvoiceFactory())
        {
        }

        public SubscriptionRenewalService(
            ICustomerRepository customerRepository,
            ISubscriptionPlanRepository planRepository,
            IRenewalRequestValidator validator,
            IBillingGateway billingGateway,
            IDiscountCalculator discountCalculator,
            ISupportFeeCalculator supportFeeCalculator,
            IPaymentFeeCalculator paymentFeeCalculator,
            ITaxCalculator taxCalculator,
            IRenewalInvoiceFactory invoiceFactory)
        {
            _customerRepository = customerRepository;
            _planRepository = planRepository;
            _validator = validator;
            _billingGateway = billingGateway;
            _discountCalculator = discountCalculator;
            _supportFeeCalculator = supportFeeCalculator;
            _paymentFeeCalculator = paymentFeeCalculator;
            _taxCalculator = taxCalculator;
            _invoiceFactory = invoiceFactory;
        }

        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            _validator.Validate(customerId, planCode, seatCount, paymentMethod);

            string normalizedPlanCode = planCode.Trim().ToUpperInvariant();
            string normalizedPaymentMethod = paymentMethod.Trim().ToUpperInvariant();

            var customer = _customerRepository.GetById(customerId);
            var plan = _planRepository.GetByCode(normalizedPlanCode);

            if (!customer.IsActive)
            {
                throw new InvalidOperationException("Inactive customers cannot renew subscriptions");
            }

            decimal baseAmount = (plan.MonthlyPricePerSeat * seatCount * 12m) + plan.SetupFee;

            var discountResult = _discountCalculator.Calculate(
                customer,
                plan,
                baseAmount,
                seatCount,
                useLoyaltyPoints);

            decimal subtotalAfterDiscount = baseAmount - discountResult.DiscountAmount;
            string notes = discountResult.Notes;

            if (subtotalAfterDiscount < 300m)
            {
                subtotalAfterDiscount = 300m;
                notes += "minimum discounted subtotal applied; ";
            }

            decimal supportFee = _supportFeeCalculator.Calculate(normalizedPlanCode, includePremiumSupport);
            if (includePremiumSupport)
            {
                notes += "premium support included; ";
            }

            decimal paymentFee = _paymentFeeCalculator.Calculate(normalizedPaymentMethod, subtotalAfterDiscount, supportFee);

            if (normalizedPaymentMethod == "CARD")
            {
                notes += "card payment fee; ";
            }
            else if (normalizedPaymentMethod == "BANK_TRANSFER")
            {
                notes += "bank transfer fee; ";
            }
            else if (normalizedPaymentMethod == "PAYPAL")
            {
                notes += "paypal fee; ";
            }
            else if (normalizedPaymentMethod == "INVOICE")
            {
                notes += "invoice payment; ";
            }

            decimal taxRate = _taxCalculator.GetTaxRate(customer.Country);
            decimal taxBase = subtotalAfterDiscount + supportFee + paymentFee;
            decimal taxAmount = taxBase * taxRate;
            decimal finalAmount = taxBase + taxAmount;

            if (finalAmount < 500m)
            {
                finalAmount = 500m;
                notes += "minimum invoice amount applied; ";
            }

            var pricingResult = new PricingResult
            {
                BaseAmount = baseAmount,
                DiscountAmount = discountResult.DiscountAmount,
                SubtotalAfterDiscount = subtotalAfterDiscount,
                SupportFee = supportFee,
                PaymentFee = paymentFee,
                TaxAmount = taxAmount,
                FinalAmount = finalAmount,
                Notes = notes
            };

            var invoice = _invoiceFactory.Create(
                customer,
                normalizedPlanCode,
                normalizedPaymentMethod,
                customerId,
                seatCount,
                pricingResult);

            _billingGateway.SaveInvoice(invoice);

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                string subject = "Subscription renewal invoice";
                string body =
                    $"Hello {customer.FullName}, your renewal for plan {normalizedPlanCode} " +
                    $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

                _billingGateway.SendEmail(customer.Email, subject, body);
            }

            return invoice;
        }
    }
}