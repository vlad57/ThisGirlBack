using System;
using API.Common;

namespace API.Services.Interfaces;

public interface IEmailService
{
    Task<Result<bool>> SendConfirmationEmailAsync(string email, string username, string confirmationToken);
}
