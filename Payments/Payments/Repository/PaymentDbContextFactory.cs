using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Payments.Repository
{
    public class PaymentDbContextFactory : IDesignTimeDbContextFactory<PaymentDbContext>
    {
        public PaymentDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PaymentDbContext>();
            
            // Use the same connection string as in appsettings.json
            optionsBuilder.UseNpgsql("Host=localhost;Database=PaymentsDb;Username=postgres;Password=postgres;Port=5432");

            return new PaymentDbContext(optionsBuilder.Options);
        }
    }
}
