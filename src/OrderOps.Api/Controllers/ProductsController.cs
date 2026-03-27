using Microsoft.AspNetCore.Mvc;
using OrderOps.Api.DTOs;
using OrderOps.Api.Services;

namespace OrderOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;

    public ProductsController(ProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductResponse>>> GetAll()
    {
        var products = await _service.GetAllAsync();
        return Ok(products);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(CreateProductRequest request)
    {
        var product = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = product.Id }, product);
    }
}
