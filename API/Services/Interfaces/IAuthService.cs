using System;
using API.Common;
using API.Models;
using API.Models.DTOs.Auth;

namespace API.Services.Interfaces;

public interface IAuthService
{
    Task<Result<bool>> RegisterAsync(RegisterRequest request);
    Task<bool> ConfirmEmailAsync(string email, string token);
}
