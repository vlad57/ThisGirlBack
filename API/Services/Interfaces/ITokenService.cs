using System;

namespace API.Services.Interfaces;

public interface ITokenService
{
    string GenerateEmailConfirmationToken();
    bool ValidateEmailConfirmationToken(string token, DateTime? tokenExpiry);
}
