namespace Commerce.Engine.Contracts
{
    public interface IPaymentProcessor
    {
        bool ProcessCreditCard(string customerName, string creditCard, string expirationDate, double amount);
    }
}