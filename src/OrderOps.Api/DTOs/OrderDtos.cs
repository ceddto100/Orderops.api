namespace OrderOps.Api.DTOs;

public record CreateOrderRequest(int CustomerId, List<OrderItemRequest> Items);

public record OrderItemRequest(int ProductId, int Quantity);

public record OrderResponse(
    int Id,
    int CustomerId,
    string CustomerName,
    DateTime OrderDate,
    string Status,
    decimal TotalAmount,
    List<OrderItemResponse> Items
);

public record OrderItemResponse(int Id, int ProductId, string ProductName, int Quantity, decimal UnitPrice);
