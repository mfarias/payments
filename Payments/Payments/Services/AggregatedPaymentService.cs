using Payments.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.Services
{
    public class AggregatedPaymentService : IAggregatedPaymentService
    {
        private readonly IRepository<AggregatedPayment> _aggregatedPaymentRepository;

        public AggregatedPaymentService(IRepository<AggregatedPayment> aggregatedPaymentRepository)
        {
            _aggregatedPaymentRepository = aggregatedPaymentRepository;
        }

        public async Task<AggregatedPayment> GetAggregatedPaymentByIdAsync(Guid id)
        {
            var aggregatedPayment = await _aggregatedPaymentRepository.GetByIdAsync(id);
            if (aggregatedPayment == null)
            {
                throw new KeyNotFoundException($"AggregatedPayment with ID {id} not found.");
            }
            return aggregatedPayment;
        }

        public async Task<IEnumerable<AggregatedPayment>> GetAllAggregatedPaymentsAsync()
        {
            var aggregatedPayments = await _aggregatedPaymentRepository.GetAllAsync();
            return aggregatedPayments.OrderByDescending(ap => ap.Date);
        }

        public async Task<IEnumerable<AggregatedPayment>> GetAggregatedPaymentsByMethodAsync(PaymentMethod paymentMethod)
        {
            var aggregatedPayments = await _aggregatedPaymentRepository.FindAsync(ap => ap.PaymentMethod == paymentMethod);
            return aggregatedPayments.OrderByDescending(ap => ap.Date);
        }

        public async Task<IEnumerable<AggregatedPayment>> GetAggregatedPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var aggregatedPayments = await _aggregatedPaymentRepository.FindAsync(ap => ap.Date >= startDate && ap.Date <= endDate);
            return aggregatedPayments.OrderByDescending(ap => ap.Date);
        }

        public async Task<AggregatedPayment> CreateAggregatedPaymentAsync(AggregatedPayment aggregatedPayment)
        {
            // Business logic: Ensure required fields
            if (aggregatedPayment.Id == Guid.Empty)
            {
                aggregatedPayment.Id = Guid.NewGuid();
            }

            if (aggregatedPayment.Date == default)
            {
                aggregatedPayment.Date = DateTime.UtcNow;
            }

            // Business validation
            if (aggregatedPayment.Amount <= 0)
            {
                throw new ArgumentException("Aggregated payment amount must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(aggregatedPayment.Currency))
            {
                aggregatedPayment.Currency = "USD"; // Default currency
            }

            return await _aggregatedPaymentRepository.AddAsync(aggregatedPayment);
        }

        public async Task<AggregatedPayment> UpdateAggregatedPaymentAsync(AggregatedPayment aggregatedPayment)
        {
            // Check if aggregated payment exists
            var existingAggregatedPayment = await _aggregatedPaymentRepository.GetByIdAsync(aggregatedPayment.Id);
            if (existingAggregatedPayment == null)
            {
                throw new KeyNotFoundException($"AggregatedPayment with ID {aggregatedPayment.Id} not found.");
            }

            // Business validation
            if (aggregatedPayment.Amount <= 0)
            {
                throw new ArgumentException("Aggregated payment amount must be greater than zero.");
            }

            return await _aggregatedPaymentRepository.UpdateAsync(aggregatedPayment);
        }

        public async Task<bool> DeleteAggregatedPaymentAsync(Guid id)
        {
            return await _aggregatedPaymentRepository.DeleteAsync(id);
        }
    }
}
