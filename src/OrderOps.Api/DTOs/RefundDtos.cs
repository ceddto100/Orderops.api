namespace OrderOps.Api.DTOs;

public record CreateRefundRequest(int PaymentId, decimal Amount, string Reason);

public record RefundResponse(int Id, int PaymentId, decimal Amount, string Reason, DateTime CreatedAt);
