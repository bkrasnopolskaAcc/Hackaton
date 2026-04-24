using System.Security.Claims;
using CarRetailSystem.Api.Modules.Sales.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRetailSystem.Api.Modules.Sales;

[ApiController]
[Route("sales")]
[Authorize]
public class SalesController(SalesService salesService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Admin,Salesperson")]
    public async Task<IActionResult> CreateSale(CreateSaleRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userName = User.FindFirstValue(ClaimTypes.Name)!;

        try
        {
            var sale = await salesService.CreateOrderAsync(request, userId, userName);
            return CreatedAtAction(nameof(GetSale), new { id = sale.SalesID }, sale);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetSale(int id)
    {
        var sale = await salesService.GetSaleAsync(id);
        return sale is null ? NotFound() : Ok(sale);
    }
}
