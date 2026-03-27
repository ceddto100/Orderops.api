using Microsoft.EntityFrameworkCore;
using OrderOps.Api.Data;
using OrderOps.Api.DTOs;
using OrderOps.Api.Models;

namespace OrderOps.Api.Services;

public class PaymentService
{
    private readonly OrderOpsDbContext _db;

    public PaymentService(OrderOpsDbContext db)
    {
        _db = db;
    }

    public async Task<PaymentResponse> CreateAsync(CreatePaymentRequest request)
    {
        var order = await _db.Orders
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId);

        if (order is null)
            throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");

        if (request.Amount <= 0)
            throw new ArgumentException("Payment amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(request.PaymentType))
            throw new ArgumentException("Payment type is required.");

        var totalPaid = order.Payments.Sum(p => p.Amount);
        var remaining = order.TotalAmount - totalPaid;

        if (request.Amount > remaining)
            throw new InvalidOperationException(
                $"Payment of {request.Amount:C} exceeds remaining balance of {remaining:C}.");

        var payment = new Payment
        {
            OrderId = request.OrderId,
            Amount = request.Amount,
            Status = "Completed",
            PaymentType = request.PaymentType.Trim()
        };

        _db.Payments.Add(payment);

        // Update order status if fully paid
        if (totalPaid + request.Amount >= order.TotalAmount)
        {
            order.Status = "Paid";
        }

        await _db.SaveChangesAsync();

        return new PaymentResponse(
            payment.Id, payment.OrderId, payment.Amount,
            payment.Status, payment.PaymentType, payment.CreatedAt);
    }
}
