namespace CarRetailSystem.Api.Modules.Reports.Dtos;

public record MonthlySalesRow(int Year, int Month, string MonthName, int SalesCount, decimal TotalRevenue, decimal AvgPrice);
public record InventoryStatusRow(string Make, string Model, int Year, int Stock, decimal Price);
public record TopSellerRow(int CarID, string Make, string Model, int Year, int UnitsSold, decimal TotalRevenue);
