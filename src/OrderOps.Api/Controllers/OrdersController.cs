using Microsoft.AspNetCore.Mvc;
using OrderOps.Api.DTOs;
using OrderOps.Api.Services;

namespace OrderOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _service;

    public OrdersController(OrderService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderResponse>>> GetAll()
    {
        var orders = await _service.GetAllAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponse>> GetById(int id)
    {
        var order = await _service.GetByIdAsync(id);
        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create(CreateOrderRequest request)
    {
        var order = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }
}
