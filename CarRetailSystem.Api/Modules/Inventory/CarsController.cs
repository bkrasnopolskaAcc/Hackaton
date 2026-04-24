using System.Security.Claims;
using CarRetailSystem.Api.Modules.Inventory.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRetailSystem.Api.Modules.Inventory;

[ApiController]
[Route("cars")]
[Authorize]
public class CarsController(InventoryService inventoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCars(
        [FromQuery] string? make,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool inStock = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        => Ok(await inventoryService.GetCarsAsync(make, maxPrice, inStock, page, pageSize));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCar(int id)
    {
        var car = await inventoryService.GetCarAsync(id);
        return car is null ? NotFound() : Ok(car);
    }

    [HttpGet("{id:int}/stock")]
    public async Task<IActionResult> GetStock(int id)
        => Ok(new { carId = id, stock = await inventoryService.GetStockAsync(id) });

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCar(CreateCarRequest request)
    {
        var car = await inventoryService.CreateCarAsync(request, CurrentUser());
        return CreatedAtAction(nameof(GetCar), new { id = car.CarID }, car);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCar(int id, UpdateCarRequest request)
    {
        var car = await inventoryService.UpdateCarAsync(id, request, CurrentUser());
        return car is null ? NotFound() : Ok(car);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCar(int id)
    {
        var deleted = await inventoryService.DeleteCarAsync(id, CurrentUser());
        return deleted ? NoContent() : NotFound();
    }

    private string CurrentUser() => User.FindFirstValue(ClaimTypes.Name) ?? "system";
}
