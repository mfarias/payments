namespace Payments
{
    public class Payment
    {
        public Guid Id { get; set; } = new Guid();
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public PaymentMethod PaymentMethod { get; set; }
    }
}
