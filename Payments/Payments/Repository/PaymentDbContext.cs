using Microsoft.EntityFrameworkCore;

namespace Payments.Repository
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
        }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<AggregatedPayment> AggregatedPayments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Payment entity
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount)
                    .HasPrecision(18, 2)
                    .IsRequired();
                entity.Property(e => e.Currency)
                    .HasMaxLength(3)
                    .IsRequired();
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                entity.Property(e => e.PaymentMethod)
                    .IsRequired();
            });

            // Configure AggregatedPayment entity
            modelBuilder.Entity<AggregatedPayment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount)
                    .HasPrecision(18, 2)
                    .IsRequired();
                entity.Property(e => e.Currency)
                    .HasMaxLength(3)
                    .IsRequired();
                entity.Property(e => e.Date)
                    .IsRequired();
                entity.Property(e => e.PaymentMethod)
                    .IsRequired();
            });
        }
    }
}
