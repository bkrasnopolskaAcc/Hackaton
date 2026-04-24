using CarRetailSystem.Api.Infrastructure.Data;
using CarRetailSystem.Api.Modules.Customers.Models;
using CarRetailSystem.Api.Modules.Inventory.Models;
using CarRetailSystem.Api.Modules.Sales;
using CarRetailSystem.Api.Modules.Sales.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CarRetailSystem.Tests.Services;

public class SalesServiceTests
{
    private static AppDbContext CreateInMemoryDb()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new AppDbContext(opts);
    }

    private static SalesService CreateService(AppDbContext db)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Business:TaxRate"] = "0.10" })
            .Build();
        return new SalesService(db, config);
    }

    [Fact]
    public void CalculateTax_Returns10Percent()
    {
        var db = CreateInMemoryDb();
        var service = CreateService(db);
        Assert.Equal(2500m, service.CalculateTax(25000m));
    }

    [Fact]
    public void CalculateTotal_ReturnsPriceWithTax()
    {
        var db = CreateInMemoryDb();
        var service = CreateService(db);
        Assert.Equal(27500m, service.CalculateTotal(25000m));
    }

    [Fact]
    public async Task CreateOrderAsync_ThrowsWhenOutOfStock()
    {
        var db = CreateInMemoryDb();
        db.Cars.Add(new Car { CarID = 1, Make = "Toyota", Model = "Camry", Year = 2022, VIN = "VIN001", Price = 25000, Stock = 0, IsActive = true });
        db.Customers.Add(new Customer { CustomerID = 1, FirstName = "Jan", LastName = "Kowalski", Email = "jan@test.com", IsActive = true });
        await db.SaveChangesAsync();

        var service = CreateService(db);
        var request = new CreateSaleRequest(1, 1, 25000m, null, null);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateOrderAsync(request, 1, "testuser"));
    }

    [Fact]
    public async Task CreateOrderAsync_DecrementsStock()
    {
        var db = CreateInMemoryDb();
        db.Cars.Add(new Car { CarID = 1, Make = "Toyota", Model = "Camry", Year = 2022, VIN = "VIN001", Price = 25000, Stock = 3, IsActive = true });
        db.Customers.Add(new Customer { CustomerID = 1, FirstName = "Jan", LastName = "Kowalski", Email = "jan@test.com", IsActive = true });
        await db.SaveChangesAsync();

        var service = CreateService(db);
        var request = new CreateSaleRequest(1, 1, 25000m, "Cash", null);
        await service.CreateOrderAsync(request, 1, "testuser");

        var car = await db.Cars.FindAsync(1);
        Assert.Equal(2, car!.Stock);
    }

    [Fact]
    public async Task CreateOrderAsync_WritesInventoryLog()
    {
        var db = CreateInMemoryDb();
        db.Cars.Add(new Car { CarID = 1, Make = "Toyota", Model = "Camry", Year = 2022, VIN = "VIN001", Price = 25000, Stock = 2, IsActive = true });
        db.Customers.Add(new Customer { CustomerID = 1, FirstName = "Jan", LastName = "Kowalski", Email = "jan@test.com", IsActive = true });
        await db.SaveChangesAsync();

        var service = CreateService(db);
        await service.CreateOrderAsync(new CreateSaleRequest(1, 1, 25000m, null, null), 1, "testuser");

        var log = await db.InventoryLogs.FirstOrDefaultAsync(l => l.Action == "SALE");
        Assert.NotNull(log);
        Assert.Equal(1, log.CarID);
    }
}
