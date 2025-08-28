namespace Payments.Models.DTO
{
    /// <summary>
    /// Request model for creating a payment
    /// </summary>
    /// <param name="Amount">Payment amount</param>
    /// <param name="Currency">Currency code (default: USD)</param>
    /// <param name="PaymentMethod">Payment method</param>
    public record CreatePaymentRequest(
        decimal Amount,
        string? Currency,
        PaymentMethod PaymentMethod
    );
}