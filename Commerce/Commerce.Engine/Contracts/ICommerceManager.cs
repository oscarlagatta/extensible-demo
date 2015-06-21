using Commerce.Common.DataModels;

namespace Commerce.Engine.Contracts
{
    public interface ICommerceManager
    {
        void ProcessOrder(OrderData orderData);
    }
}