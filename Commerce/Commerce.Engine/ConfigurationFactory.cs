using System;
using System.Configuration;
using Commerce.Common;
using Commerce.Engine.Configuration;
using Commerce.Engine.Contracts;

namespace Commerce.Engine
{
    public class ConfigurationFactory : IConfigurationFactory
    {
        public ConfigurationFactory()
        {
            CommerceEngineConfigurationSection config = ConfigurationManager.GetSection("commerceEngine") as CommerceEngineConfigurationSection;

            if (config != null)
            {
                _paymentProcessor = Activator.CreateInstance(Type.GetType(config.PaymentProcessor.type)) as IPaymentProcessor;
                _paymentProcessor.LoginName = config.PaymentProcessor.LoginName;
                _paymentProcessor.Password = config.PaymentProcessor.Password;

                _mailer = Activator.CreateInstance(Type.GetType(config.Mailer.type)) as IMailer;
                _mailer.FromAddres = config.Mailer.FromAddress;
                _mailer.SmptServer = config.Mailer.SmtpServer;
            }
        }

        IPaymentProcessor _paymentProcessor;
        IMailer _mailer;


        public IPaymentProcessor GetPaymentProcessor()
        {
            return _paymentProcessor;
        }

        public IMailer GetMailer()
        {
            return _mailer;
        }
    }
}