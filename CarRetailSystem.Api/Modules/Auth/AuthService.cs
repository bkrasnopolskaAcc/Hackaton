using CarRetailSystem.Api.Infrastructure.Auth;
using CarRetailSystem.Api.Infrastructure.Data;
using CarRetailSystem.Api.Modules.Auth.Dtos;
using CarRetailSystem.Api.Modules.Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CarRetailSystem.Api.Modules.Auth;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    JwtTokenService jwt,
    IDistributedCache cache)
{
    public async Task<TokenResponse?> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByNameAsync(request.UserName);
        if (user is null || !user.IsActive) return null;

        bool valid;

        // On-login password migration: validate legacy plain text, then bcrypt it
        if (!user.PasswordMigrated && user.LegacyPassword is not null)
        {
            valid = user.LegacyPassword == request.Password;
            if (valid)
            {
                await userManager.AddPasswordAsync(user, request.Password);
                user.LegacyPassword = null;
                user.PasswordMigrated = true;
                await userManager.UpdateAsync(user);
            }
        }
        else
        {
            valid = await userManager.CheckPasswordAsync(user, request.Password);
        }

        if (!valid) return null;

        user.LastLoginDate = DateTime.UtcNow;
        await userManager.UpdateAsync(user);

        return await IssueTokensAsync(user);
    }

    public async Task<TokenResponse?> RefreshAsync(string refreshToken)
    {
        var cached = await cache.GetStringAsync($"refresh:{refreshToken}");
        if (cached is null) return null;

        var userId = JsonSerializer.Deserialize<int>(cached);
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || !user.IsActive) return null;

        await cache.RemoveAsync($"refresh:{refreshToken}");
        return await IssueTokensAsync(user);
    }

    public async Task RevokeAsync(string refreshToken)
        => await cache.RemoveAsync($"refresh:{refreshToken}");

    private async Task<TokenResponse> IssueTokensAsync(ApplicationUser user)
    {
        var accessToken = jwt.GenerateAccessToken(user);
        var refreshToken = JwtTokenService.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddMinutes(15);

        await cache.SetStringAsync(
            $"refresh:{refreshToken}",
            JsonSerializer.Serialize(user.Id),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7) }
        );

        return new TokenResponse(accessToken, refreshToken, expiresAt, user.Role);
    }
}
