using System;
using System.Security.Cryptography;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class TokenService : ITokenService
{
    public string GenerateEmailConfirmationToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    public bool ValidateEmailConfirmationToken(string token, DateTime? tokenExpiry)
    {
        return !string.IsNullOrEmpty(token) && 
               tokenExpiry.HasValue && 
               tokenExpiry.Value > DateTime.UtcNow;
    }
}
