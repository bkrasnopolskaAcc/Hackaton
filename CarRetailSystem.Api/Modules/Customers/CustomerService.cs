using CarRetailSystem.Api.Infrastructure.Data;
using CarRetailSystem.Api.Modules.Customers.Dtos;
using CarRetailSystem.Api.Modules.Customers.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRetailSystem.Api.Modules.Customers;

public class CustomerService(AppDbContext db)
{
    public async Task<CustomerListResponse> GetCustomersAsync(string? search, int page, int pageSize)
    {
        var query = db.Customers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.FirstName.Contains(search) || c.LastName.Contains(search) || c.Email.Contains(search));

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(c => ToResponse(c))
            .ToListAsync();

        return new CustomerListResponse(items, total, page, pageSize);
    }

    public async Task<CustomerResponse?> GetCustomerAsync(int id)
        => await db.Customers.Where(c => c.CustomerID == id).Select(c => ToResponse(c)).FirstOrDefaultAsync();

    public async Task<CustomerResponse> CreateCustomerAsync(CreateCustomerRequest request)
    {
        var customer = new Customer
        {
            FirstName = request.FirstName, LastName = request.LastName,
            Email = request.Email, Phone = request.Phone,
            Address = request.Address, City = request.City,
            State = request.State, ZipCode = request.ZipCode
        };
        db.Customers.Add(customer);
        await db.SaveChangesAsync();
        return ToResponse(customer);
    }

    public async Task<CustomerResponse?> UpdateCustomerAsync(int id, UpdateCustomerRequest request)
    {
        var customer = await db.Customers.FindAsync(id);
        if (customer is null) return null;

        customer.FirstName = request.FirstName; customer.LastName = request.LastName;
        customer.Email = request.Email; customer.Phone = request.Phone;
        customer.Address = request.Address; customer.City = request.City;
        customer.State = request.State; customer.ZipCode = request.ZipCode;

        await db.SaveChangesAsync();
        return ToResponse(customer);
    }

    public async Task<bool> DeleteCustomerAsync(int id)
    {
        var customer = await db.Customers.FindAsync(id);
        if (customer is null) return false;
        customer.IsActive = false;
        await db.SaveChangesAsync();
        return true;
    }

    private static CustomerResponse ToResponse(Customer c)
        => new(c.CustomerID, c.FirstName, c.LastName, c.Email, c.Phone, c.Address, c.City, c.State, c.ZipCode, c.RegisteredDate);
}
