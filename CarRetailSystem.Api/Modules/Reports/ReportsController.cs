using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRetailSystem.Api.Modules.Reports;

[ApiController]
[Route("reports")]
[Authorize]
public class ReportsController(ReportService reportService) : ControllerBase
{
    [HttpGet("monthly-sales")]
    public async Task<IActionResult> MonthlySales([FromQuery] int? year)
        => Ok(await reportService.GetMonthlySalesAsync(year ?? DateTime.UtcNow.Year));

    [HttpGet("inventory")]
    public async Task<IActionResult> InventoryStatus()
        => Ok(await reportService.GetInventoryStatusAsync());

    [HttpGet("top-sellers")]
    public async Task<IActionResult> TopSellers([FromQuery] int? year, [FromQuery] int top = 10)
        => Ok(await reportService.GetTopSellersAsync(year ?? DateTime.UtcNow.Year, top));
}
