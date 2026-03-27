using Microsoft.EntityFrameworkCore;
using OrderOps.Api.Data;
using OrderOps.Api.DTOs;
using OrderOps.Api.Models;

namespace OrderOps.Api.Services;

public class OrderService
{
    private readonly OrderOpsDbContext _db;

    public OrderService(OrderOpsDbContext db)
    {
        _db = db;
    }

    public async Task<List<OrderResponse>> GetAllAsync()
    {
        return await _db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => MapToResponse(o))
            .ToListAsync();
    }

    public async Task<OrderResponse> GetByIdAsync(int id)
    {
        var order = await _db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
            throw new KeyNotFoundException($"Order with ID {id} not found.");

        return MapToResponse(order);
    }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request)
    {
        if (request.Items == null || request.Items.Count == 0)
            throw new ArgumentException("Order must contain at least one item.");

        // Verify customer exists
        var customer = await _db.Customers.FindAsync(request.CustomerId);
        if (customer is null)
            throw new KeyNotFoundException($"Customer with ID {request.CustomerId} not found.");

        // Gather all product IDs and load them
        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        // Validate all products exist and have sufficient stock
        var orderItems = new List<OrderItem>();
        decimal totalAmount = 0;

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
                throw new ArgumentException($"Quantity must be greater than zero for product ID {item.ProductId}.");

            if (!products.TryGetValue(item.ProductId, out var product))
                throw new KeyNotFoundException($"Product with ID {item.ProductId} not found.");

            if (product.StockQuantity < item.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}, Requested: {item.Quantity}.");

            var unitPrice = product.Price;
            totalAmount += unitPrice * item.Quantity;

            orderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = unitPrice
            });
        }

        // Reduce stock for each product
        foreach (var item in request.Items)
        {
            products[item.ProductId].StockQuantity -= item.Quantity;
        }

        var order = new Order
        {
            CustomerId = request.CustomerId,
            Status = "Pending",
            TotalAmount = totalAmount,
            Items = orderItems
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        // Reload with navigation properties for the response
        return await GetByIdAsync(order.Id);
    }

    private static OrderResponse MapToResponse(Order o)
    {
        return new OrderResponse(
            o.Id,
            o.CustomerId,
            o.Customer.Name,
            o.OrderDate,
            o.Status,
            o.TotalAmount,
            o.Items.Select(i => new OrderItemResponse(
                i.Id,
                i.ProductId,
                i.Product.Name,
                i.Quantity,
                i.UnitPrice
            )).ToList()
        );
    }
}
