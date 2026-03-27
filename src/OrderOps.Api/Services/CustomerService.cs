using Microsoft.EntityFrameworkCore;
using OrderOps.Api.Data;
using OrderOps.Api.DTOs;
using OrderOps.Api.Models;

namespace OrderOps.Api.Services;

public class CustomerService
{
    private readonly OrderOpsDbContext _db;

    public CustomerService(OrderOpsDbContext db)
    {
        _db = db;
    }

    public async Task<List<CustomerResponse>> GetAllAsync()
    {
        return await _db.Customers
            .OrderBy(c => c.Name)
            .Select(c => new CustomerResponse(c.Id, c.Name, c.Email, c.CreatedAt))
            .ToListAsync();
    }

    public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Customer name is required.");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Customer email is required.");

        var exists = await _db.Customers.AnyAsync(c => c.Email == request.Email);
        if (exists)
            throw new InvalidOperationException($"A customer with email '{request.Email}' already exists.");

        var customer = new Customer
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim().ToLower()
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        return new CustomerResponse(customer.Id, customer.Name, customer.Email, customer.CreatedAt);
    }
}
