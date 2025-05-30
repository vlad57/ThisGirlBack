using API.Templates.Emails.Auth.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace API.Templates.Emails.Auth.en
{
    public class RegisterConfirmationEmailViewModelModel : PageModel, IRegisterConfirmationEmailViewModelModel
    {
        public string Username { get; set; } = string.Empty;
        public string ConfirmationUrl { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public string SupportEmail { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
    }
}
