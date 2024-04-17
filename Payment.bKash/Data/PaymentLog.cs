namespace Payment.bKash.Data
{
    public class PaymentLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string paymentID { get; set; }
        public string status { get; set; }
        public string amount { get; set; }
        public string token { get; set; }
        public string trxNo { get; set; }
    }
}
