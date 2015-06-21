using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commerce.Engine;
using Commerce.Engine.Contracts;
using Microsoft.Practices.Unity;

namespace Commerce.TestClient
{
    class Program
    {
        static void Main(string[] args)
        {

            #region Change 1
            // The client is still using the concrete so here is where I need to start doing 
            // my registration for the dependency injection container.
            // So here I no longer want to instantiate the StoreRepository concretely
            // Instead I want to instantiate the dependency injection container
            // and let it take care for me.
            //StoreRepository storeRepository = new StoreRepository();
            //storeRepository.Initialize();
            #endregion

            IUnityContainer container = new UnityContainer();
            // register my types
            container.RegisterType<IStoreRepository, StoreRepository>()
                     .RegisterType<ICommerceManager, CommerceManager>()
                     .RegisterType<IPaymentProcessor, PaymentProcessor>()
                     .RegisterType<IMailer, Mailer>();

            #region Change 2.0
            // In order for the container resolve process kick start 
            // I have to ask the container for the CommerceEngine class, 
            // instead of hardcoding like this
            // CommerceManager commerceEngine = new CommerceManager(storeRepository);
            // commerceEngine.ProcessOrder(orderData);
            #endregion

            OrderData orderData = new OrderData()
            {
                CustomerEmail = "oscarlagatta@gmail.com",
                LineItems = new List<OrderLineItemData>()
                {
                   new OrderLineItemData() { Sku = 102, PurchasePrice = 479.00, Quantity = 1 },
                   new OrderLineItemData() { Sku = 101, PurchasePrice = 659.00, Quantity = 2 },
                   new OrderLineItemData() { Sku = 103, PurchasePrice = 529.00, Quantity = 1 },
                   new OrderLineItemData() { Sku = 104, PurchasePrice = 609.00, Quantity = 3 }
                },
                CreditCard = "1234123412341234",
                ExpirationDate = "1217"
            };

            #region Change 2.1
            // need to ask the container for the CommerceManager class,
            // we ask the container to resolve the class CommerceManager
            // we also need to abstract the CommerceManager into an interface
            // CommerceManager commerceEngine = new CommerceManager(storeRepository);
            // This is the step we take to make the CommerceManger fully testable, 
            // because now we can mock an instance of the IStoreRepository,
            // so that doesn't hit the database and inject it manually in a unit test
            // so we can test the ProcessOrder method of the CommerceManager
            // Resolving the top level class (the CommerceManager) will resolve 
            // all the dependencies rescursively down the chain
            #endregion

            ICommerceManager commerceEngine = container.Resolve<ICommerceManager>();

            #region Change 3
            // Change into implementation the PaymentProcessor and the Mailer 
            // in the CommerceManager
            #endregion

            commerceEngine.ProcessOrder(orderData);

            Console.WriteLine();
            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();
        }


    }
}
