using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using CarRetailSystem.Api.Common.Middleware;
using CarRetailSystem.Api.Infrastructure.Auth;
using CarRetailSystem.Api.Infrastructure.Data;
using CarRetailSystem.Api.Modules.Auth;
using CarRetailSystem.Api.Modules.Auth.Models;
using CarRetailSystem.Api.Modules.Customers;
using CarRetailSystem.Api.Modules.Inventory;
using CarRetailSystem.Api.Modules.Reports;
using CarRetailSystem.Api.Modules.Sales;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build())
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Database
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(opt =>
{
    opt.Password.RequiredLength = 12;
    opt.Password.RequireUppercase = true;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireDigit = true;
    opt.Password.RequireNonAlphanumeric = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// JWT
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("Jwt:Secret is not configured.");
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

// Rate limiting on login endpoint
builder.Services.AddRateLimiter(opt =>
{
    opt.AddFixedWindowLimiter("login", limiterOpt =>
    {
        limiterOpt.PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:LoginMaxAttempts", 5);
        limiterOpt.Window = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("RateLimiting:LoginWindowSeconds", 60));
        limiterOpt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOpt.QueueLimit = 0;
    });
    opt.RejectionStatusCode = 429;
});

// Caching (in-memory for dev; swap for Redis in prod via config)
builder.Services.AddDistributedMemoryCache();

// Services
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<SalesService>();
builder.Services.AddScoped<ReportService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT support
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CarRetailSystem API",
        Version = "v1",
        Description = "Modernized Car Retail System — ASP.NET Core 8"
    });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CarRetailSystem v1"));
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Apply EF migrations on startup (dev only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow })
   .AllowAnonymous();

await app.RunAsync();

public partial class Program { }
