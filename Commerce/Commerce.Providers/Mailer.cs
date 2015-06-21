using Commerce.Common;
using Commerce.Common.DataModels;
using System;

namespace Commerce.Providers
{
    public class Mailer : IMailer
    {
        public void SendInvoicEmail(OrderData orderData)
        {
            Console.WriteLine("The Following is your invoice for the order, sent to {0}", orderData.CustomerEmail);
        }

        public void SendRejectionEmail(OrderData orderData)
        {
            Console.WriteLine("I'm sorry {0}, your order could not be processed at this time", orderData.CustomerEmail);
        }


        public string FromAddres { get; set; }

        public string SmptServer { get; set; }
    }
}