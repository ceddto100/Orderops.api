using Microsoft.EntityFrameworkCore;
using OrderOps.Api.Data;
using OrderOps.Api.DTOs;
using OrderOps.Api.Models;

namespace OrderOps.Api.Services;

public class ProductService
{
    private readonly OrderOpsDbContext _db;

    public ProductService(OrderOpsDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProductResponse>> GetAllAsync()
    {
        return await _db.Products
            .OrderBy(p => p.Name)
            .Select(p => new ProductResponse(p.Id, p.Name, p.SKU, p.Price, p.StockQuantity, p.CreatedAt))
            .ToListAsync();
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Product name is required.");

        if (string.IsNullOrWhiteSpace(request.SKU))
            throw new ArgumentException("Product SKU is required.");

        if (request.Price <= 0)
            throw new ArgumentException("Price must be greater than zero.");

        if (request.StockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative.");

        var skuExists = await _db.Products.AnyAsync(p => p.SKU == request.SKU);
        if (skuExists)
            throw new InvalidOperationException($"A product with SKU '{request.SKU}' already exists.");

        var product = new Product
        {
            Name = request.Name.Trim(),
            SKU = request.SKU.Trim().ToUpper(),
            Price = request.Price,
            StockQuantity = request.StockQuantity
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return new ProductResponse(product.Id, product.Name, product.SKU, product.Price, product.StockQuantity, product.CreatedAt);
    }
}
