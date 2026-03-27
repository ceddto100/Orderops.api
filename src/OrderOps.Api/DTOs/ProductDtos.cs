namespace OrderOps.Api.DTOs;

public record CreateProductRequest(string Name, string SKU, decimal Price, int StockQuantity);

public record ProductResponse(int Id, string Name, string SKU, decimal Price, int StockQuantity, DateTime CreatedAt);
