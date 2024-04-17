namespace Payment.bKash.Models
{
    public class PaymentCreateVm
    {
        public string senderBkashNo { get; set; }
        public string amount { get; set; }
        public string token { get; set; }
    }
}
