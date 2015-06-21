using Commerce.Common.DataModels;
namespace Commerce.Common
{
    public interface IMailer
    {
        void SendInvoicEmail(OrderData orderData);
        void SendRejectionEmail(OrderData orderData);

        /* Added from the configuration change */
        string FromAddres { get; set; }
        string SmptServer { get; set; }

    }
}