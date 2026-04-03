namespace LegacyRenewalApp.Abstractions
{
    public interface ICustomerRepository
    {
        Customer GetById(int customerId);
    }
}