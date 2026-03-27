using Microsoft.EntityFrameworkCore;
using OrderOps.Api.Data;
using OrderOps.Api.Middleware;
using OrderOps.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<OrderOpsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<RefundService>();
builder.Services.AddScoped<ReportService>();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "OrderOps API", Version = "v1" });
});

var app = builder.Build();

// Global error handling
app.UseMiddleware<ExceptionMiddleware>();

// Swagger (always on for dev convenience)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderOps API v1");
    c.RoutePrefix = string.Empty; // Swagger at root URL
});

app.MapControllers();

app.Run();
