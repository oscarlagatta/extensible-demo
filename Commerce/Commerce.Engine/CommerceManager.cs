using System;
using System.Configuration;
using System.Linq;
using System.Transactions;
using Commerce.Common;
using Commerce.Common.DataModels;
using Commerce.Engine.Configuration;
using Commerce.Engine.Contracts;
using Commerce.Engine.Entities;

namespace Commerce.Engine
{
    public class CommerceManager : ICommerceManager
    {
        public CommerceManager(IStoreRepository storeRepository,
            IConfigurationFactory configurationFactory)
        {
            _storeRepository = storeRepository;
            // we need to bring the configuration from the config file

            #region Breaks testability
            /*
             * We are using providers which is great
             * We're hardcoding how we obtain them NOT GREAT :( 
             * A unit test shouldn't be locked to this behaviour
             * 
             * SOLUTION
             * --------
             * Wrap it in a factory class
             *  get the configuration
             *  fill the provider class with their config-based values
             *  
             * Abstract out the factory and inject it into the engine
             */
            //CommerceEngineConfigurationSection config = ConfigurationManager.GetSection("commerceEngine") as CommerceEngineConfigurationSection;


            //if (config != null)
            //{
            //    _paymentProcessor = Activator.CreateInstance(Type.GetType(config.PaymentProcessor.type)) as IPaymentProcessor;
            //    _paymentProcessor.LoginName = config.PaymentProcessor.LoginName;
            //    _paymentProcessor.Password = config.PaymentProcessor.Password;

            //    _mailer = Activator.CreateInstance(Type.GetType(config.Mailer.type)) as IMailer;
            //    _mailer.FromAddres = config.Mailer.FromAddress;
            //    _mailer.SmptServer = config.Mailer.SmtpServer;
            //}
            #endregion

            #region Fixed for testability 

            _paymentProcessor = configurationFactory.GetPaymentProcessor();
            _mailer = configurationFactory.GetMailer();

            #endregion

        }
        
        IStoreRepository _storeRepository;
        IPaymentProcessor _paymentProcessor;
        IMailer _mailer;

        public void ProcessOrder(OrderData orderData)
        {
            try
            {
                // System.Transactions
                using (TransactionScope scope = new TransactionScope())
                {
                    Customer customer = _storeRepository.GetCustomerByEmail(orderData.CustomerEmail);

                    if (customer == null)
                    {
                        throw new ApplicationException(string.Format("No customer on file with email {0}.", orderData.CustomerEmail));
                    }
                    // Decrease product inventory
                    foreach (OrderLineItemData lineItem in orderData.LineItems)
                    {
                        Product product =
                            _storeRepository.Products.Where(item => item.Sku == lineItem.Sku).FirstOrDefault();

                        Inventory inventoryOnHand =
                            _storeRepository.ProductInventory.Where(item => item.Sku == lineItem.Sku).FirstOrDefault();

                        if (inventoryOnHand == null)
                        {
                            throw new ApplicationException(string.Format("Error attempting to determine on-hand inventory quantity for product {0}.", lineItem.Sku));
                        }

                        if (inventoryOnHand.QuantityInStock < lineItem.Quantity)
                        {
                            throw new ApplicationException(string.Format("Not enough quantity on-hand to satisfy product {0} purchase of {1} units.", lineItem.Sku, lineItem.Quantity));

                        }
                        inventoryOnHand.QuantityInStock -= lineItem.Quantity;
                        Console.WriteLine("Inventory for product {0} reduced by {1} units.", lineItem.Sku, lineItem.Quantity);
                    }

                    // Update customer information
                    foreach (OrderLineItemData lineItem in orderData.LineItems)
                    {
                        for (int i = 0; i < lineItem.Quantity; i++)
                        {
                            customer.Purchases.Add(new PurchasedItem()
                            {
                                Sku = lineItem.Sku,
                                PurchasePrice = lineItem.PurchasePrice,
                                PurchasedOn = DateTime.Now
                            });
                        }

                        Console.WriteLine("Added {0} unit(s) or product {1} to customer's purchase history.", lineItem.Quantity, lineItem.Sku);
                    }

                    // process customer credit card
                    double amount = 0;
                    foreach (OrderLineItemData lineItem in orderData.LineItems)
                    {
                        amount += (lineItem.Quantity * lineItem.PurchasePrice);
                    }

                    #region Change 3
                    // remove instantiation
                    // PaymentProcessor paymentProcessor = new PaymentProcessor();
                    // use variable  _paymentProcessor
                    #endregion

                    bool paymentSuccess = _paymentProcessor.ProcessCreditCard(customer.Name, orderData.CreditCard,
                        orderData.ExpirationDate, amount);
                    if (!paymentSuccess)
                    {
                        throw new ApplicationException(string.Format("Credit card {0} could not be processed.", orderData.CreditCard));
                    }

                    scope.Complete();
                }

                #region Change 3
                // remove instantiation
                // Mailer mailer = new Mailer();
                // use variable _mailer
                #endregion

                _mailer.SendInvoicEmail(orderData);
            }
            catch (Exception)
            {
                //Mailer mailer = new Mailer();
                _mailer.SendRejectionEmail(orderData);
                throw;
            }
        }

    }
}