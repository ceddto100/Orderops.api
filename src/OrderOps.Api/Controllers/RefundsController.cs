using Microsoft.AspNetCore.Mvc;
using OrderOps.Api.DTOs;
using OrderOps.Api.Services;

namespace OrderOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RefundsController : ControllerBase
{
    private readonly RefundService _service;

    public RefundsController(RefundService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<RefundResponse>> Create(CreateRefundRequest request)
    {
        var refund = await _service.CreateAsync(request);
        return CreatedAtAction(null, new { id = refund.Id }, refund);
    }
}
