using System.ComponentModel.DataAnnotations;

namespace CarRetailSystem.Api.Modules.Sales.Dtos;

public record CreateSaleRequest(
    [Range(1, int.MaxValue)] int CustomerID,
    [Range(1, int.MaxValue)] int CarID,
    [Range(0.01, double.MaxValue)] decimal SalesPrice,
    [MaxLength(50)] string? PaymentMethod,
    [MaxLength(500)] string? Notes
);

public record SaleResponse(
    int SalesID, int CustomerID, string CustomerName,
    int CarID, string CarDescription,
    decimal SalesPrice, decimal Tax, decimal Total,
    DateTime SalesDate, string SalespersonName,
    string? PaymentMethod, string? Notes
);
