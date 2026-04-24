using System.ComponentModel.DataAnnotations;

namespace CarRetailSystem.Api.Modules.Inventory.Dtos;

public record CarResponse(
    int CarID, string Make, string Model, int Year,
    string? Color, string VIN, decimal Price, int Stock, string? Description
);

public record CarListResponse(
    IEnumerable<CarResponse> Items, int TotalCount, int Page, int PageSize
);

public record CreateCarRequest(
    [Required, MaxLength(50)] string Make,
    [Required, MaxLength(50)] string Model,
    [Range(1900, 2100)] int Year,
    [MaxLength(30)] string? Color,
    [Required, MaxLength(17)] string VIN,
    [Range(0.01, double.MaxValue)] decimal Price,
    [Range(0, int.MaxValue)] int Stock,
    [MaxLength(500)] string? Description
);

public record UpdateCarRequest(
    [Required, MaxLength(50)] string Make,
    [Required, MaxLength(50)] string Model,
    [Range(1900, 2100)] int Year,
    [MaxLength(30)] string? Color,
    [Range(0.01, double.MaxValue)] decimal Price,
    [Range(0, int.MaxValue)] int Stock,
    [MaxLength(500)] string? Description
);
