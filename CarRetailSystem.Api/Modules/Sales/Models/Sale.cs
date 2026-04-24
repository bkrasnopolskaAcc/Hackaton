using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CarRetailSystem.Api.Modules.Auth.Models;
using CarRetailSystem.Api.Modules.Customers.Models;
using CarRetailSystem.Api.Modules.Inventory.Models;

namespace CarRetailSystem.Api.Modules.Sales.Models;

public class Sale
{
    [Key]
    public int SalesID { get; set; }

    public int CustomerID { get; set; }
    public Customer Customer { get; set; } = null!;

    public int CarID { get; set; }
    public Car Car { get; set; } = null!;

    [Column(TypeName = "decimal(10,2)")]
    public decimal SalesPrice { get; set; }

    public DateTime SalesDate { get; set; } = DateTime.UtcNow;

    public int SalespersonID { get; set; }
    public ApplicationUser Salesperson { get; set; } = null!;

    [MaxLength(50)]
    public string? PaymentMethod { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
