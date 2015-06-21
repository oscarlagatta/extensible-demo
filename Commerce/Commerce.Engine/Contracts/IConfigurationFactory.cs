using Commerce.Common;

namespace Commerce.Engine.Contracts
{
    public interface IConfigurationFactory
    {
        IPaymentProcessor GetPaymentProcessor();
        IMailer GetMailer();
    }
}