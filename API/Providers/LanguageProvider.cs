using System;
using Microsoft.AspNetCore.Localization;

namespace API.Providers;

public class LanguageProvider : RequestCultureProvider
{
    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        // Chercher la langue dans le header Accept-Language personnalisé
        var acceptLanguageHeader = httpContext.Request.Headers["Accept-Language"].FirstOrDefault();
        
        if (!string.IsNullOrEmpty(acceptLanguageHeader))
        {
            var supportedCultures = new[] { "fr", "en" };
            
            if (supportedCultures.Contains(acceptLanguageHeader.ToLower()))
            {
                return Task.FromResult<ProviderCultureResult?>(
                    new ProviderCultureResult(acceptLanguageHeader, acceptLanguageHeader));
            }
        }

        return Task.FromResult<ProviderCultureResult?>(null);
    }
}
