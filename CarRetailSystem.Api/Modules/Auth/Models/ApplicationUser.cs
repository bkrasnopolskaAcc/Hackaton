using Microsoft.AspNetCore.Identity;

namespace CarRetailSystem.Api.Modules.Auth.Models;

public class ApplicationUser : IdentityUser<int>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = "Salesperson"; // Admin, Salesperson, Manager
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginDate { get; set; }

    // Migration support: legacy plain-text password (null once migrated)
    public string? LegacyPassword { get; set; }
    public bool PasswordMigrated { get; set; } = false;
}
