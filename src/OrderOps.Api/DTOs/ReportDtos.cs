namespace OrderOps.Api.DTOs;

public record DailyRevenueReport(DateTime Date, decimal TotalRevenue, int OrderCount);

public record TopSellingProductReport(int ProductId, string ProductName, int TotalQuantitySold, decimal TotalRevenue);
