using Payments.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<Payment> _paymentRepository;

        public PaymentService(IRepository<Payment> paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        #region Payment Operations

        public async Task<Payment> GetPaymentByIdAsync(Guid id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with ID {id} not found.");
            }
            return payment;
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();
            return payments.OrderByDescending(p => p.CreatedAt);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByMethodAsync(PaymentMethod paymentMethod)
        {
            var payments = await _paymentRepository.FindAsync(p => p.PaymentMethod == paymentMethod);
            return payments.OrderByDescending(p => p.CreatedAt);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var payments = await _paymentRepository.FindAsync(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate);
            return payments.OrderByDescending(p => p.CreatedAt);
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            // Business logic: Ensure required fields
            if (payment.Id == Guid.Empty)
            {
                payment.Id = Guid.NewGuid();
            }

            if (payment.CreatedAt == default)
            {
                payment.CreatedAt = DateTime.UtcNow;
            }

            // Business validation
            if (payment.Amount <= 0)
            {
                throw new ArgumentException("Payment amount must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(payment.Currency))
            {
                payment.Currency = "USD"; // Default currency
            }

            return await _paymentRepository.AddAsync(payment);
        }

        public async Task<Payment> UpdatePaymentAsync(Payment payment)
        {
            // Check if payment exists
            var existingPayment = await _paymentRepository.GetByIdAsync(payment.Id);
            if (existingPayment == null)
            {
                throw new KeyNotFoundException($"Payment with ID {payment.Id} not found.");
            }

            // Business validation
            if (payment.Amount <= 0)
            {
                throw new ArgumentException("Payment amount must be greater than zero.");
            }

            return await _paymentRepository.UpdateAsync(payment);
        }

        public async Task<bool> DeletePaymentAsync(Guid id)
        {
            return await _paymentRepository.DeleteAsync(id);
        }

        #endregion
    }
}
