using CarRetailSystem.Api.Infrastructure.Data;
using CarRetailSystem.Api.Modules.Inventory.Dtos;
using CarRetailSystem.Api.Modules.Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRetailSystem.Api.Modules.Inventory;

public class InventoryService(AppDbContext db)
{
    public async Task<CarListResponse> GetCarsAsync(string? make, decimal? maxPrice, bool inStockOnly, int page, int pageSize)
    {
        var query = db.Cars.AsQueryable();

        if (!string.IsNullOrWhiteSpace(make))
            query = query.Where(c => c.Make.Contains(make));
        if (maxPrice.HasValue)
            query = query.Where(c => c.Price <= maxPrice.Value);
        if (inStockOnly)
            query = query.Where(c => c.Stock > 0);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(c => c.Make).ThenBy(c => c.Model)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(c => ToResponse(c))
            .ToListAsync();

        return new CarListResponse(items, total, page, pageSize);
    }

    public async Task<CarResponse?> GetCarAsync(int id)
        => await db.Cars.Where(c => c.CarID == id).Select(c => ToResponse(c)).FirstOrDefaultAsync();

    public async Task<int> GetStockAsync(int id)
        => await db.Cars.Where(c => c.CarID == id).Select(c => c.Stock).FirstOrDefaultAsync();

    public async Task<CarResponse> CreateCarAsync(CreateCarRequest request, string createdBy)
    {
        var car = new Car
        {
            Make = request.Make, Model = request.Model, Year = request.Year,
            Color = request.Color, VIN = request.VIN, Price = request.Price,
            Stock = request.Stock, Description = request.Description
        };
        db.Cars.Add(car);
        await db.SaveChangesAsync();
        await WriteAuditLogAsync(car.CarID, "ADD", request.Stock, createdBy, "Initial stock");
        return ToResponse(car);
    }

    public async Task<CarResponse?> UpdateCarAsync(int id, UpdateCarRequest request, string updatedBy)
    {
        var car = await db.Cars.FindAsync(id);
        if (car is null) return null;

        var stockDelta = request.Stock - car.Stock;
        car.Make = request.Make; car.Model = request.Model; car.Year = request.Year;
        car.Color = request.Color; car.Price = request.Price;
        car.Stock = request.Stock; car.Description = request.Description;
        car.LastModifiedDate = DateTime.UtcNow;

        await db.SaveChangesAsync();

        if (stockDelta != 0)
            await WriteAuditLogAsync(id, "UPDATE", stockDelta, updatedBy, $"Stock adjusted by {stockDelta}");

        return ToResponse(car);
    }

    public async Task<bool> DeleteCarAsync(int id, string deletedBy)
    {
        var car = await db.Cars.FindAsync(id);
        if (car is null) return false;
        car.IsActive = false;
        car.LastModifiedDate = DateTime.UtcNow;
        await db.SaveChangesAsync();
        await WriteAuditLogAsync(id, "DELETE", 0, deletedBy, "Soft deleted");
        return true;
    }

    public async Task UpdateStockAsync(int carId, int quantity, string changedBy, string notes)
    {
        var car = await db.Cars.FindAsync(carId);
        if (car is null) return;
        car.Stock -= quantity;
        car.LastModifiedDate = DateTime.UtcNow;
        await WriteAuditLogAsync(carId, "SALE", quantity, changedBy, notes);
    }

    private async Task WriteAuditLogAsync(int carId, string action, int quantity, string changedBy, string? notes)
    {
        db.InventoryLogs.Add(new InventoryLog
        {
            CarID = carId, Action = action, Quantity = quantity,
            ChangedBy = changedBy, Notes = notes, ChangedDate = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }

    private static CarResponse ToResponse(Car c)
        => new(c.CarID, c.Make, c.Model, c.Year, c.Color, c.VIN, c.Price, c.Stock, c.Description);
}
