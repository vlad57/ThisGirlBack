using System;
using API.Common;
using API.Data;
using API.Models;
using API.Models.DTOs.Auth;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _configuration;
    private readonly ITranslationService _translationService;
    private readonly IEmailService _emailService;
    private readonly ITokenService _tokenService;

    public AuthService(
        DatabaseContext context,
        IConfiguration configuration,
        ITranslationService translationService,
        IEmailService emailService,
        ITokenService tokenService
    )
    {
        _context = context;
        _configuration = configuration;
        _translationService = translationService;
        _emailService = emailService;
        _tokenService = tokenService;
    }

    public async Task<Result<User>> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Username == request.Username || u.Email == request.Email))
        {
            return Result<User>.Fail(_translationService.GetTranslation("Errors.Auth.UserExists"), 400);
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var confirmationToken = _tokenService.GenerateEmailConfirmationToken();
            var tokenExpiry = DateTime.UtcNow.AddHours(24);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                createdAt = DateTime.Today,
                EmailConfirmationToken = confirmationToken,
                EmailConfirmationTokenExpiry = tokenExpiry,
                IsEmailConfirmed = false
            };

            var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);
            user.PasswordHash = hashedPassword;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var emailSent = await _emailService.SendConfirmationEmailAsync(
                user.Email,
                user.Username,
                confirmationToken
            );

            if (!emailSent.Data)
            {
                await transaction.RollbackAsync();
                return Result<User>.Fail(_translationService.GetTranslation("Errors.Auth.EmailSendFailed"), 500);
            }

            await transaction.CommitAsync();

            return Result<User>.Ok(user);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Result<User>.Fail(ex.Message, 500);
        }
    }
    
    public async Task<bool> ConfirmEmailAsync(string email, string token)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => 
                u.Email == email.ToLower().Trim() && 
                u.EmailConfirmationToken == token);

            if (user == null || !_tokenService.ValidateEmailConfirmationToken(token, user.EmailConfirmationTokenExpiry))
            {
                return false;
            }

            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpiry = null;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
