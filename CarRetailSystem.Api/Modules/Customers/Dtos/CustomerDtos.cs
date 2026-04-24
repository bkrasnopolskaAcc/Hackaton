using System.ComponentModel.DataAnnotations;

namespace CarRetailSystem.Api.Modules.Customers.Dtos;

public record CustomerResponse(
    int CustomerID, string FirstName, string LastName, string Email,
    string? Phone, string? Address, string? City, string? State, string? ZipCode, DateTime RegisteredDate
);

public record CustomerListResponse(IEnumerable<CustomerResponse> Items, int TotalCount, int Page, int PageSize);

public record CreateCustomerRequest(
    [Required, MaxLength(50)] string FirstName,
    [Required, MaxLength(50)] string LastName,
    [Required, EmailAddress, MaxLength(100)] string Email,
    [MaxLength(20)] string? Phone,
    [MaxLength(200)] string? Address,
    [MaxLength(100)] string? City,
    [MaxLength(50)] string? State,
    [MaxLength(10)] string? ZipCode
);

public record UpdateCustomerRequest(
    [Required, MaxLength(50)] string FirstName,
    [Required, MaxLength(50)] string LastName,
    [Required, EmailAddress, MaxLength(100)] string Email,
    [MaxLength(20)] string? Phone,
    [MaxLength(200)] string? Address,
    [MaxLength(100)] string? City,
    [MaxLength(50)] string? State,
    [MaxLength(10)] string? ZipCode
);
