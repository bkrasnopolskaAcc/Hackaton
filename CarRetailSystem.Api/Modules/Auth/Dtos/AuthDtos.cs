using System.ComponentModel.DataAnnotations;

namespace CarRetailSystem.Api.Modules.Auth.Dtos;

public record LoginRequest(
    [Required] string UserName,
    [Required] string Password
);

public record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string Role
);

public record RefreshRequest(
    [Required] string RefreshToken
);
