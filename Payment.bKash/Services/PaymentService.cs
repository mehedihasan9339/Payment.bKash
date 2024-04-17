

using Microsoft.EntityFrameworkCore;
using Payment.bKash.Data;

namespace Payment.bKash.Services
{
    public class PaymentService: IPayment
    {
        private readonly databaseContext _context;

        public PaymentService(databaseContext context)
        {
            _context = context;
        }


        public async Task<PaymentLog> GetPaymentLogByPaymentId(string paymentId)
        {
            var data = await _context.PaymentLogs.Where(x => x.paymentID == paymentId).AsNoTracking().FirstOrDefaultAsync();

            return data;
        }


        public async Task<int> SavePaymentLogs(PaymentLog model)
        {
            try
            {
                if (model.Id > 0)
                {
                    _context.PaymentLogs.Update(model);
                }
                else
                {
                    _context.PaymentLogs.Add(model);
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
