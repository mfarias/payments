using Microsoft.AspNetCore.Mvc;
using Payments.Services;
using Payments.Models.DTO;

namespace Payments.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IAggregatedPaymentService _aggregatedPaymentService;

        public PaymentController(IPaymentService paymentService, IAggregatedPaymentService aggregatedPaymentService)
        {
            _paymentService = paymentService;
            _aggregatedPaymentService = aggregatedPaymentService;
        }

        /// <summary>
        /// Creates a new payment
        /// </summary>
        /// <param name="request">Payment creation request</param>
        /// <returns>Created payment</returns>
        [HttpPost]
        public async Task<ActionResult<Payment>> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var payment = new Payment
                {
                    Amount = request.Amount,
                    Currency = request.Currency ?? "USD",
                    PaymentMethod = request.PaymentMethod
                };

                var createdPayment = await _paymentService.CreatePaymentAsync(payment);
                return CreatedAtAction(nameof(GetPayment), new { id = createdPayment.Id }, createdPayment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while creating the payment", details = ex.Message });
            }
        }

        /// <summary>
        /// Gets a payment by ID
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <returns>Payment details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(Guid id)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByIdAsync(id);
                
                if (payment == null)
                {
                    return NotFound(new { error = "Payment not found", id });
                }

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving the payment", details = ex.Message });
            }
        }

        /// <summary>
        /// Gets aggregated payments by date range
        /// </summary>
        /// <param name="startDate">Start date (YYYY-MM-DD format)</param>
        /// <param name="endDate">End date (YYYY-MM-DD format)</param>
        /// <returns>List of aggregated payments</returns>
        [HttpGet("aggregated")]
        public async Task<ActionResult<IEnumerable<AggregatedPayment>>> GetAggregatedPayments(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new { error = "Start date cannot be later than end date" });
                }

                if (startDate == default(DateTime) || endDate == default(DateTime))
                {
                    return BadRequest(new { error = "Both startDate and endDate are required in YYYY-MM-DD format" });
                }

                var aggregatedPayments = await _aggregatedPaymentService.GetAggregatedPaymentsByDateRangeAsync(startDate, endDate);
                return Ok(aggregatedPayments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving aggregated payments", details = ex.Message });
            }
        }
    }
}
