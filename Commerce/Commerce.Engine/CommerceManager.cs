using System;
using System.Linq;
using System.Transactions;
using Commerce.Engine.Entities;

namespace Commerce.Engine
{
    public class CommerceManager
    {
        public CommerceManager(StoreRepository storeRepository)
        {
            this._storeRepository = storeRepository;
        }

        private StoreRepository _storeRepository;


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
                        amount += (lineItem.Quantity*lineItem.PurchasePrice);
                    }

                    PaymentProcessor paymentProcessor = new PaymentProcessor();

                    bool paymentSuccess = paymentProcessor.ProcessCreditCard(customer.Name, orderData.CreditCard,
                        orderData.ExpirationDate, amount);
                    if (!paymentSuccess)
                    {
                        throw new ApplicationException(string.Format("Credit card {0} could not be processed.", orderData.CreditCard));
                    }

                    scope.Complete();
                }

                Mailer mailer = new Mailer();
                mailer.SendInvoicEmail(orderData);
            }
            catch (Exception)
            {
                Mailer mailer  = new Mailer();
                mailer.SendRejectionEmail(orderData);
                throw;
            }
        }
    }
}