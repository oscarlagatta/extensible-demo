using System.Configuration;

namespace Commerce.Engine.Configuration
{
    public class CommerceEngineConfigurationSection : ConfigurationSection 
    {
         // paymentprocessor element and mailer element
        [ConfigurationProperty("paymentProcessor",IsRequired = true)]
        public PaymentProcessorElement PaymentProcessor
        {
            get { return (PaymentProcessorElement)base["paymentProcessor"]; }
            set { base["paymentProcessor"] = value; }
        }

        [ConfigurationProperty("mailer", IsRequired = true)]
        public MailerElement Mailer
        {
            get { return (MailerElement)base["mailer"]; }
            set { base["mailer"] = value; }
        }
    }

    public class ProviderTypeElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string name
        {
            get { return (string) base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("type", IsRequired = true, IsKey = true)]
        public string type
        {
            get { return (string)base["type"]; }
            set { base["type"] = value; }
        }

    }

    public class PaymentProcessorElement : ProviderTypeElement
    {
        [ConfigurationProperty("loginName", IsRequired = true, IsKey = true)]
        public string LoginName
        {
            get { return (string)base["loginName"]; }
            set { base["loginName"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get { return (string)base["password"]; }
            set { base["password"] = value; }
        }
    }

    public class MailerElement : ProviderTypeElement
    {
        [ConfigurationProperty("fromAddress", IsRequired = true, IsKey = true)]
        public string FromAddress
        {
            get { return (string)base["fromAddress"]; }
            set { base["fromAddress"] = value; }
        }

        [ConfigurationProperty("smtpServer", IsRequired = true)]
        public string SmtpServer
        {
            get { return (string)base["smtpServer"]; }
            set { base["smtpServer"] = value; }
        }
       
    }
}