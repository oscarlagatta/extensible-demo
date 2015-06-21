using System;

namespace Commerce.Engine
{
    public class PaymentProcessor
    {
        public bool ProcessCreditCard(string customerName, string creditCard, string expirationDate, double amount)
        {
            // process credit card using ??? Payment Gateway and return success or failure
            Console.WriteLine("Credit Card processed with ??? Payment Gateway for {0:c}.", amount);

            return true;
        }
    }
}