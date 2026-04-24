using CarRetailSystem.Api.Infrastructure.Data;
using CarRetailSystem.Api.Modules.Inventory.Models;
using CarRetailSystem.Api.Modules.Sales.Dtos;
using CarRetailSystem.Api.Modules.Sales.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRetailSystem.Api.Modules.Sales;

public class SalesService(AppDbContext db, IConfiguration config)
{
    private decimal TaxRate => config.GetValue<decimal>("Business:TaxRate", 0.10m);

    public decimal CalculateTax(decimal price) => Math.Round(price * TaxRate, 2);
    public decimal CalculateTotal(decimal price) => price + CalculateTax(price);

    public async Task<SaleResponse> CreateOrderAsync(CreateSaleRequest request, int salespersonId, string salespersonName)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();

        var car = await db.Cars.FindAsync(request.CarID)
            ?? throw new InvalidOperationException($"Car {request.CarID} not found.");
        if (car.Stock < 1)
            throw new InvalidOperationException($"Car {request.CarID} is out of stock.");

        var sale = new Sale
        {
            CustomerID = request.CustomerID, CarID = request.CarID,
            SalesPrice = request.SalesPrice, SalespersonID = salespersonId,
            PaymentMethod = request.PaymentMethod, Notes = request.Notes
        };
        db.Sales.Add(sale);
        await db.SaveChangesAsync();

        car.Stock--;
        car.LastModifiedDate = DateTime.UtcNow;

        db.InventoryLogs.Add(new InventoryLog
        {
            CarID = car.CarID, Action = "SALE", Quantity = 1,
            ChangedBy = salespersonName, Notes = $"SalesOrder_{sale.SalesID}",
            ChangedDate = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        await transaction.CommitAsync();

        var customer = await db.Customers.FindAsync(request.CustomerID);
        return ToResponse(sale, customer!.FirstName + " " + customer.LastName,
            $"{car.Year} {car.Make} {car.Model}", salespersonName);
    }

    public async Task<SaleResponse?> GetSaleAsync(int id)
    {
        var sale = await db.Sales
            .Include(s => s.Customer)
            .Include(s => s.Car)
            .Include(s => s.Salesperson)
            .FirstOrDefaultAsync(s => s.SalesID == id);

        if (sale is null) return null;
        return ToResponse(sale,
            sale.Customer.FirstName + " " + sale.Customer.LastName,
            $"{sale.Car.Year} {sale.Car.Make} {sale.Car.Model}",
            sale.Salesperson.FirstName + " " + sale.Salesperson.LastName);
    }

    public async Task<IEnumerable<SaleResponse>> GetCustomerHistoryAsync(int customerId)
    {
        return await db.Sales
            .Include(s => s.Customer)
            .Include(s => s.Car)
            .Include(s => s.Salesperson)
            .Where(s => s.CustomerID == customerId)
            .OrderByDescending(s => s.SalesDate)
            .Select(s => ToResponse(s,
                s.Customer.FirstName + " " + s.Customer.LastName,
                s.Car.Year + " " + s.Car.Make + " " + s.Car.Model,
                s.Salesperson.FirstName + " " + s.Salesperson.LastName))
            .ToListAsync();
    }

    private SaleResponse ToResponse(Sale s, string customerName, string carDesc, string salespersonName)
        => new(s.SalesID, s.CustomerID, customerName, s.CarID, carDesc,
            s.SalesPrice, CalculateTax(s.SalesPrice), CalculateTotal(s.SalesPrice),
            s.SalesDate, salespersonName, s.PaymentMethod, s.Notes);
}
