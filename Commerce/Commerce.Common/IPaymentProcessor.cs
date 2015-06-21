namespace Commerce.Common
{
    public interface IPaymentProcessor
    {
        bool ProcessCreditCard(string customerName, string creditCard, string expirationDate, double amount);

        string LoginName { get; set; }
        string Password { get; set; }

    }
}