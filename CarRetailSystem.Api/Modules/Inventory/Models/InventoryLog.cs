using System.ComponentModel.DataAnnotations;

namespace CarRetailSystem.Api.Modules.Inventory.Models;

public class InventoryLog
{
    [Key]
    public int LogID { get; set; }

    public int CarID { get; set; }
    public Car Car { get; set; } = null!;

    [Required, MaxLength(20)]
    public string Action { get; set; } = string.Empty; // ADD, SALE, UPDATE, DELETE, AUDIT

    public int Quantity { get; set; }
    public DateTime ChangedDate { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? ChangedBy { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
