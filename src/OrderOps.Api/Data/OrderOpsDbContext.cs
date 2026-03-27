using Microsoft.EntityFrameworkCore;
using OrderOps.Api.Models;

namespace OrderOps.Api.Data;

public class OrderOpsDbContext : DbContext
{
    public OrderOpsDbContext(DbContextOptions<OrderOpsDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Refund> Refunds => Set<Refund>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure decimal precision to avoid SQL Server truncation warnings
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Refund>()
            .Property(r => r.Amount)
            .HasColumnType("decimal(18,2)");

        // Relationships
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Order)
            .WithMany(o => o.Payments)
            .HasForeignKey(p => p.OrderId);

        modelBuilder.Entity<Refund>()
            .HasOne(r => r.Payment)
            .WithMany(p => p.Refunds)
            .HasForeignKey(r => r.PaymentId);

        // Unique constraint on Product SKU
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.SKU)
            .IsUnique();

        // Unique constraint on Customer Email
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Email)
            .IsUnique();

        // Seed data
        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, Name = "Alice Johnson", Email = "alice@example.com", CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 2, Name = "Bob Smith", Email = "bob@example.com", CreatedAt = new DateTime(2024, 2, 20, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 3, Name = "Carol Davis", Email = "carol@example.com", CreatedAt = new DateTime(2024, 3, 10, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Wireless Mouse", SKU = "WM-001", Price = 29.99m, StockQuantity = 150, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 2, Name = "Mechanical Keyboard", SKU = "MK-002", Price = 89.99m, StockQuantity = 75, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 3, Name = "USB-C Hub", SKU = "UH-003", Price = 49.99m, StockQuantity = 200, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 4, Name = "Monitor Stand", SKU = "MS-004", Price = 39.99m, StockQuantity = 100, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 5, Name = "Webcam HD", SKU = "WC-005", Price = 59.99m, StockQuantity = 60, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
