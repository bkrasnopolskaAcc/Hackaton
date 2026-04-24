using CarRetailSystem.Api.Modules.Auth.Models;
using CarRetailSystem.Api.Modules.Customers.Models;
using CarRetailSystem.Api.Modules.Inventory.Models;
using CarRetailSystem.Api.Modules.Sales.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarRetailSystem.Api.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>(options)
{
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<InventoryLog> InventoryLogs => Set<InventoryLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Car>(e =>
        {
            e.HasIndex(c => c.VIN).IsUnique();
            e.HasIndex(c => c.Make);
            e.HasIndex(c => c.Stock);
            e.HasQueryFilter(c => c.IsActive);
        });

        builder.Entity<Customer>(e =>
        {
            e.HasIndex(c => c.Email);
            e.HasQueryFilter(c => c.IsActive);
        });

        builder.Entity<Sale>(e =>
        {
            e.HasIndex(s => s.SalesDate);
            e.HasIndex(s => s.CustomerID);
            e.HasOne(s => s.Customer).WithMany(c => c.Sales).HasForeignKey(s => s.CustomerID);
            e.HasOne(s => s.Car).WithMany(c => c.Sales).HasForeignKey(s => s.CarID);
            e.HasOne(s => s.Salesperson).WithMany().HasForeignKey(s => s.SalespersonID);
        });

        builder.Entity<InventoryLog>(e =>
        {
            e.HasOne(l => l.Car).WithMany(c => c.InventoryLogs).HasForeignKey(l => l.CarID);
        });

        SeedData(builder);
    }

    private static void SeedData(ModelBuilder builder)
    {
        builder.Entity<Car>().HasData(
            new Car { CarID = 1, Make = "Toyota", Model = "Camry", Year = 2022, Color = "Silver", VIN = "1HGBH41JXMN109186", Price = 28500, Stock = 3, IsActive = true, CreatedDate = new DateTime(2024, 1, 1) },
            new Car { CarID = 2, Make = "Honda", Model = "Civic", Year = 2023, Color = "Blue", VIN = "2HGFE2F59NH123456", Price = 24000, Stock = 5, IsActive = true, CreatedDate = new DateTime(2024, 1, 1) },
            new Car { CarID = 3, Make = "Ford", Model = "Mustang", Year = 2022, Color = "Red", VIN = "1FA6P8TH5N5100001", Price = 42000, Stock = 2, IsActive = true, CreatedDate = new DateTime(2024, 1, 1) },
            new Car { CarID = 4, Make = "BMW", Model = "3 Series", Year = 2023, Color = "Black", VIN = "WBA5R1C50KAE12345", Price = 55000, Stock = 1, IsActive = true, CreatedDate = new DateTime(2024, 1, 1) }
        );

        builder.Entity<Customer>().HasData(
            new Customer { CustomerID = 1, FirstName = "Jan", LastName = "Kowalski", Email = "jan.kowalski@email.com", Phone = "555-0101", City = "Warsaw", IsActive = true, RegisteredDate = new DateTime(2024, 1, 1) },
            new Customer { CustomerID = 2, FirstName = "Anna", LastName = "Nowak", Email = "anna.nowak@email.com", Phone = "555-0102", City = "Krakow", IsActive = true, RegisteredDate = new DateTime(2024, 1, 1) }
        );
    }
}
