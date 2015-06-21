using Commerce.Common;
using System;

namespace Commerce.Providers
{
    public class PaymentProcessor : IPaymentProcessor
    {
        public bool ProcessCreditCard(string customerName, string creditCard, string expirationDate, double amount)
        {
            // process credit card using ??? Payment Gateway and return success or failure
            Console.WriteLine("Credit Card processed with ??? Payment Gateway for {0:c}.", amount);

            return true;
        }

        public string LoginName { get; set; }

        public string Password { get; set; }
    }
}