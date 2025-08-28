namespace Payments
{
    public class AggregatedPayment
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public DateTime Date { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
