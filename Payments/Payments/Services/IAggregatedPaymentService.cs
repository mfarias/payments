using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Payments.Services
{
    public interface IAggregatedPaymentService
    {
        Task<AggregatedPayment> GetAggregatedPaymentByIdAsync(Guid id);
        Task<IEnumerable<AggregatedPayment>> GetAllAggregatedPaymentsAsync();
        Task<IEnumerable<AggregatedPayment>> GetAggregatedPaymentsByMethodAsync(PaymentMethod paymentMethod);
        Task<IEnumerable<AggregatedPayment>> GetAggregatedPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<AggregatedPayment> CreateAggregatedPaymentAsync(AggregatedPayment aggregatedPayment);
        Task<AggregatedPayment> UpdateAggregatedPaymentAsync(AggregatedPayment aggregatedPayment);
        Task<bool> DeleteAggregatedPaymentAsync(Guid id);
    }
}
