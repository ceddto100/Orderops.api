namespace OrderOps.Api.DTOs;

public record CreatePaymentRequest(int OrderId, decimal Amount, string PaymentType);

public record PaymentResponse(int Id, int OrderId, decimal Amount, string Status, string PaymentType, DateTime CreatedAt);
