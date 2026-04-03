namespace LegacyRenewalApp.Abstractions
{
    public interface ISubscriptionPlanRepository
    {
        SubscriptionPlan GetByCode(string code);
    }
}