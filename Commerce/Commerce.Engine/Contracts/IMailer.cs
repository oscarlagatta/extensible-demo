namespace Commerce.Engine.Contracts
{
    public interface IMailer
    {
        void SendInvoicEmail(OrderData orderData);
        void SendRejectionEmail(OrderData orderData);
    }
}