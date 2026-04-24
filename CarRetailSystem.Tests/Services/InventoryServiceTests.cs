using CarRetailSystem.Api.Infrastructure.Data;
using CarRetailSystem.Api.Modules.Inventory;
using CarRetailSystem.Api.Modules.Inventory.Dtos;
using CarRetailSystem.Api.Modules.Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRetailSystem.Tests.Services;

public class InventoryServiceTests
{
    private static AppDbContext CreateInMemoryDb()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    [Fact]
    public async Task GetCarsAsync_FiltersInStock()
    {
        var db = CreateInMemoryDb();
        db.Cars.AddRange(
            new Car { CarID = 1, Make = "Toyota", Model = "Camry", Year = 2022, VIN = "V1", Price = 25000, Stock = 2, IsActive = true },
            new Car { CarID = 2, Make = "Honda", Model = "Civic", Year = 2022, VIN = "V2", Price = 20000, Stock = 0, IsActive = true }
        );
        await db.SaveChangesAsync();

        var service = new InventoryService(db);
        var result = await service.GetCarsAsync(null, null, inStockOnly: true, page: 1, pageSize: 10);

        Assert.Equal(1, result.TotalCount);
        Assert.All(result.Items, c => Assert.True(c.Stock > 0));
    }

    [Fact]
    public async Task GetCarsAsync_FiltersByMake()
    {
        var db = CreateInMemoryDb();
        db.Cars.AddRange(
            new Car { CarID = 1, Make = "Toyota", Model = "Camry", Year = 2022, VIN = "V1", Price = 25000, Stock = 2, IsActive = true },
            new Car { CarID = 2, Make = "Honda", Model = "Civic", Year = 2022, VIN = "V2", Price = 20000, Stock = 1, IsActive = true }
        );
        await db.SaveChangesAsync();

        var service = new InventoryService(db);
        var result = await service.GetCarsAsync("Toyota", null, false, 1, 10);

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Toyota", result.Items.First().Make);
    }

    [Fact]
    public async Task CreateCarAsync_SetsIsActiveTrue()
    {
        var db = CreateInMemoryDb();
        var service = new InventoryService(db);
        var request = new CreateCarRequest("BMW", "X5", 2023, "Black", "VIN123456789012345", 65000, 1, null);

        var result = await service.CreateCarAsync(request, "admin");

        var saved = await db.Cars.FindAsync(result.CarID);
        Assert.True(saved!.IsActive);
    }

    [Fact]
    public async Task DeleteCarAsync_SoftDeletes()
    {
        var db = CreateInMemoryDb();
        db.Cars.Add(new Car { CarID = 5, Make = "Audi", Model = "A4", Year = 2021, VIN = "AUDIN12345", Price = 45000, Stock = 1, IsActive = true });
        await db.SaveChangesAsync();

        var service = new InventoryService(db);
        var deleted = await service.DeleteCarAsync(5, "admin");

        Assert.True(deleted);
        var car = await db.Cars.IgnoreQueryFilters().FirstAsync(c => c.CarID == 5);
        Assert.False(car.IsActive);
    }
}
