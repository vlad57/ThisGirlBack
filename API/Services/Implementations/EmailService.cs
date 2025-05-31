using System;
using System.Net;
using System.Net.Mail;
using API.Common;
using API.Constants;
using API.Services.Interfaces;
using API.Templates.Emails.Auth.Interfaces;

namespace API.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;
    private readonly IRazorViewToStringRenderer _razorViewRenderer;
    private readonly ITranslationService _translationService;

    public EmailService(
        IConfiguration config,
        ILogger<EmailService> logger,
        IRazorViewToStringRenderer razorViewRenderer,
        ITranslationService translationService
    )
    {
        _config = config;
        _logger = logger;
        _razorViewRenderer = razorViewRenderer;
        _translationService = translationService;
    }

    public async Task<Result<bool>> SendConfirmationEmailAsync(string email, string username, string confirmationToken)
    {
        try
        {
            var smtpSettings = _config.GetSection("SmtpClient");
            var baseUrl = _config["BaseUrl"] ?? "https://localhost:5001";

            using var client = new SmtpClient(smtpSettings["SMTPServerHost"], int.Parse(smtpSettings["SMTPServerPort"]!))
            {
                Credentials = new NetworkCredential(smtpSettings["SMTPUsername"], smtpSettings["SMTPPassword"]),
                EnableSsl = bool.Parse(smtpSettings["SMTPUseSsl"]!)
            };

            var confirmationUrl = $"{baseUrl}/api/auth/confirm-email?email={Uri.EscapeDataString(email)}&token={confirmationToken}";

            IRegisterConfirmationEmailViewModelModel emailViewModelPath = new Templates.Emails.Auth.en.RegisterConfirmationEmailViewModelModel();

            if (_translationService.GetCurrentCultureCode() == LanguageConstants.En)
            {
                emailViewModelPath = new Templates.Emails.Auth.en.RegisterConfirmationEmailViewModelModel();
            }

            emailViewModelPath.Username = username;
            emailViewModelPath.ConfirmationUrl = confirmationUrl;
            emailViewModelPath.ExpiryDate = DateTime.UtcNow.AddHours(24);
            emailViewModelPath.SupportEmail = smtpSettings["SupportEmail"] ?? smtpSettings["FromAddress"]!;
            emailViewModelPath.CompanyName = _config["CompanyName"] ?? "Notre Application";

            var emailBody = await _razorViewRenderer.RenderViewToStringAsync("Auth/" + _translationService.GetCurrentCultureCode() + "/RegisterConfirmationEmail", emailViewModelPath);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["FromAddress"]!, smtpSettings["FromName"]),
                Subject = _translationService.GetTranslation("Auth.Emails.Subject.ConfirmEmailAddress"),
                Body = emailBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erreur lors de l'envoi de l'email à {email}");
            return Result<bool>.Ok(false);;
        }
    }
}
