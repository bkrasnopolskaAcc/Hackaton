using CarRetailSystem.Api.Modules.Customers.Dtos;
using CarRetailSystem.Api.Modules.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRetailSystem.Api.Modules.Customers;

[ApiController]
[Route("customers")]
[Authorize]
public class CustomersController(CustomerService customerService, SalesService salesService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCustomers(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        => Ok(await customerService.GetCustomersAsync(search, page, pageSize));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCustomer(int id)
    {
        var customer = await customerService.GetCustomerAsync(id);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpGet("{id:int}/sales")]
    public async Task<IActionResult> GetCustomerSales(int id)
        => Ok(await salesService.GetCustomerHistoryAsync(id));

    [HttpPost]
    [Authorize(Roles = "Admin,Salesperson")]
    public async Task<IActionResult> CreateCustomer(CreateCustomerRequest request)
    {
        var customer = await customerService.CreateCustomerAsync(request);
        return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerID }, customer);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Salesperson")]
    public async Task<IActionResult> UpdateCustomer(int id, UpdateCustomerRequest request)
    {
        var customer = await customerService.UpdateCustomerAsync(id, request);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var deleted = await customerService.DeleteCustomerAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
