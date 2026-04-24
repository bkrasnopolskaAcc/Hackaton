using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CarRetailSystem.Api.Modules.Sales.Models;

namespace CarRetailSystem.Api.Modules.Inventory.Models;

public class Car
{
    [Key]
    public int CarID { get; set; }

    [Required, MaxLength(50)]
    public string Make { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }

    [MaxLength(30)]
    public string? Color { get; set; }

    [Required, MaxLength(17)]
    public string VIN { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    public int Stock { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedDate { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Sale> Sales { get; set; } = [];
    public ICollection<InventoryLog> InventoryLogs { get; set; } = [];
}
