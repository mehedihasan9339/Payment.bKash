namespace Payment.bKash.Context
{
    public class databaseContext : IdentityDbContext
    {
        public databaseContext(DbContextOptions<databaseContext> options) : base(options)
        {

        }

        public DbSet<PaymentLog> PaymentLogs { get; set; }
    }
}
