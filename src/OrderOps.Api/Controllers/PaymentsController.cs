using Microsoft.AspNetCore.Mvc;
using OrderOps.Api.DTOs;
using OrderOps.Api.Services;

namespace OrderOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _service;

    public PaymentsController(PaymentService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<PaymentResponse>> Create(CreatePaymentRequest request)
    {
        var payment = await _service.CreateAsync(request);
        return CreatedAtAction(null, new { id = payment.Id }, payment);
    }
}
