using Microsoft.EntityFrameworkCore;
using OrderOps.Api.Data;
using OrderOps.Api.DTOs;

namespace OrderOps.Api.Services;

public class ReportService
{
    private readonly OrderOpsDbContext _db;

    public ReportService(OrderOpsDbContext db)
    {
        _db = db;
    }

    public async Task<List<DailyRevenueReport>> GetDailyRevenueAsync()
    {
        return await _db.Payments
            .Where(p => p.Status == "Completed")
            .GroupBy(p => p.CreatedAt.Date)
            .Select(g => new DailyRevenueReport(
                g.Key,
                g.Sum(p => p.Amount),
                g.Select(p => p.OrderId).Distinct().Count()
            ))
            .OrderByDescending(r => r.Date)
            .ToListAsync();
    }

    public async Task<List<TopSellingProductReport>> GetTopSellingProductsAsync(int count = 10)
    {
        return await _db.OrderItems
            .Include(oi => oi.Product)
            .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
            .Select(g => new TopSellingProductReport(
                g.Key.ProductId,
                g.Key.Name,
                g.Sum(oi => oi.Quantity),
                g.Sum(oi => oi.Quantity * oi.UnitPrice)
            ))
            .OrderByDescending(r => r.TotalQuantitySold)
            .Take(count)
            .ToListAsync();
    }
}
