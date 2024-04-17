using Payment.bKash.Data;

namespace Payment.bKash.Services
{
    public interface IPayment
    {
        Task<PaymentLog> GetPaymentLogByPaymentId(string paymentId);
        Task<int> SavePaymentLogs(PaymentLog model);
    }
}
