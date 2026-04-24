using CarRetailSystem.Api.Infrastructure.Data;
using CarRetailSystem.Api.Modules.Reports.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CarRetailSystem.Api.Modules.Reports;

public class ReportService(AppDbContext db, IDistributedCache cache)
{
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    public async Task<IEnumerable<MonthlySalesRow>> GetMonthlySalesAsync(int year)
    {
        var key = $"report:monthly:{year}";
        var cached = await cache.GetStringAsync(key);
        if (cached is not null)
            return JsonSerializer.Deserialize<IEnumerable<MonthlySalesRow>>(cached)!;

        var result = await db.Sales
            .Where(s => s.SalesDate.Year == year)
            .GroupBy(s => new { s.SalesDate.Year, s.SalesDate.Month })
            .Select(g => new MonthlySalesRow(
                g.Key.Year, g.Key.Month,
                new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM"),
                g.Count(),
                g.Sum(s => s.SalesPrice),
                g.Average(s => s.SalesPrice)))
            .OrderBy(r => r.Month)
            .ToListAsync();

        await cache.SetStringAsync(key, JsonSerializer.Serialize(result), CacheOptions);
        return result;
    }

    public async Task<IEnumerable<InventoryStatusRow>> GetInventoryStatusAsync()
    {
        var key = "report:inventory";
        var cached = await cache.GetStringAsync(key);
        if (cached is not null)
            return JsonSerializer.Deserialize<IEnumerable<InventoryStatusRow>>(cached)!;

        var result = await db.Cars
            .OrderBy(c => c.Make).ThenBy(c => c.Model)
            .Select(c => new InventoryStatusRow(c.Make, c.Model, c.Year, c.Stock, c.Price))
            .ToListAsync();

        await cache.SetStringAsync(key, JsonSerializer.Serialize(result), CacheOptions);
        return result;
    }

    public async Task<IEnumerable<TopSellerRow>> GetTopSellersAsync(int year, int top = 10)
    {
        var key = $"report:topsellers:{year}:{top}";
        var cached = await cache.GetStringAsync(key);
        if (cached is not null)
            return JsonSerializer.Deserialize<IEnumerable<TopSellerRow>>(cached)!;

        var result = await db.Sales
            .Where(s => s.SalesDate.Year == year)
            .GroupBy(s => new { s.CarID, s.Car.Make, s.Car.Model, s.Car.Year })
            .Select(g => new TopSellerRow(
                g.Key.CarID, g.Key.Make, g.Key.Model, g.Key.Year,
                g.Count(), g.Sum(s => s.SalesPrice)))
            .OrderByDescending(r => r.UnitsSold)
            .Take(top)
            .ToListAsync();

        await cache.SetStringAsync(key, JsonSerializer.Serialize(result), CacheOptions);
        return result;
    }
}
