using Microsoft.AspNetCore.Mvc;
using OrderOps.Api.DTOs;
using OrderOps.Api.Services;

namespace OrderOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ReportService _service;

    public ReportsController(ReportService service)
    {
        _service = service;
    }

    [HttpGet("daily-revenue")]
    public async Task<ActionResult<List<DailyRevenueReport>>> DailyRevenue()
    {
        var report = await _service.GetDailyRevenueAsync();
        return Ok(report);
    }

    [HttpGet("top-products")]
    public async Task<ActionResult<List<TopSellingProductReport>>> TopProducts([FromQuery] int count = 10)
    {
        var report = await _service.GetTopSellingProductsAsync(count);
        return Ok(report);
    }
}
