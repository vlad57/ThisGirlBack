using System;

namespace API.Templates.Emails.Auth.Interfaces;

public interface IRegisterConfirmationEmailViewModelModel
{
    string Username { get; set; }
    string ConfirmationUrl { get; set; }
    DateTime ExpiryDate { get; set; }
    string SupportEmail { get; set; }
    string CompanyName { get; set; }
}
