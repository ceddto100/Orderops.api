using Microsoft.AspNetCore.Mvc;
using OrderOps.Api.DTOs;
using OrderOps.Api.Services;

namespace OrderOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _service;

    public CustomersController(CustomerService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<CustomerResponse>>> GetAll()
    {
        var customers = await _service.GetAllAsync();
        return Ok(customers);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> Create(CreateCustomerRequest request)
    {
        var customer = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = customer.Id }, customer);
    }
}
