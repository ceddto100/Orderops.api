using Microsoft.EntityFrameworkCore;
using OrderOps.Api.Data;
using OrderOps.Api.DTOs;
using OrderOps.Api.Models;

namespace OrderOps.Api.Services;

public class RefundService
{
    private readonly OrderOpsDbContext _db;

    public RefundService(OrderOpsDbContext db)
    {
        _db = db;
    }

    public async Task<RefundResponse> CreateAsync(CreateRefundRequest request)
    {
        var payment = await _db.Payments
            .Include(p => p.Refunds)
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Id == request.PaymentId);

        if (payment is null)
            throw new KeyNotFoundException($"Payment with ID {request.PaymentId} not found.");

        if (request.Amount <= 0)
            throw new ArgumentException("Refund amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(request.Reason))
            throw new ArgumentException("Refund reason is required.");

        var totalRefunded = payment.Refunds.Sum(r => r.Amount);
        var refundable = payment.Amount - totalRefunded;

        if (request.Amount > refundable)
            throw new InvalidOperationException(
                $"Refund of {request.Amount:C} exceeds refundable amount of {refundable:C}.");

        var refund = new Refund
        {
            PaymentId = request.PaymentId,
            Amount = request.Amount,
            Reason = request.Reason.Trim()
        };

        _db.Refunds.Add(refund);

        // If fully refunded, update payment and order status
        if (totalRefunded + request.Amount >= payment.Amount)
        {
            payment.Status = "Refunded";

            // Check if all payments for this order are refunded
            var allPayments = await _db.Payments
                .Include(p => p.Refunds)
                .Where(p => p.OrderId == payment.OrderId)
                .ToListAsync();

            var allFullyRefunded = allPayments.All(p =>
                p.Id == payment.Id
                    ? (totalRefunded + request.Amount >= p.Amount)
                    : (p.Refunds.Sum(r => r.Amount) >= p.Amount));

            if (allFullyRefunded)
            {
                payment.Order.Status = "Refunded";
            }
        }

        await _db.SaveChangesAsync();

        return new RefundResponse(refund.Id, refund.PaymentId, refund.Amount, refund.Reason, refund.CreatedAt);
    }
}
