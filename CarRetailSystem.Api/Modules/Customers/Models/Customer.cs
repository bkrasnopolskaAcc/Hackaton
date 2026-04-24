using System.ComponentModel.DataAnnotations;
using CarRetailSystem.Api.Modules.Sales.Models;

namespace CarRetailSystem.Api.Modules.Customers.Models;

public class Customer
{
    [Key]
    public int CustomerID { get; set; }

    [Required, MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(50)]
    public string? State { get; set; }

    [MaxLength(10)]
    public string? ZipCode { get; set; }

    public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<Sale> Sales { get; set; } = [];
}
