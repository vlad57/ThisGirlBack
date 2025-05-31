using System;
using API.Constants;
using Microsoft.AspNetCore.Localization;

namespace API.Providers;

public class LanguageProvider : RequestCultureProvider
{
    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var acceptLanguageHeader = httpContext.Request.Headers["Accept-Language"].FirstOrDefault();
    
        if (!string.IsNullOrEmpty(acceptLanguageHeader))
        {
            var supportedCultures = new[] { LanguageConstants.Fr, LanguageConstants.En };
            
            var cleanedLanguage = acceptLanguageHeader.Trim().ToLower();
            
            var primaryLanguage = cleanedLanguage.Split('-')[0];
        
            
            if (supportedCultures.Contains(primaryLanguage))
            {
                return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(primaryLanguage, primaryLanguage));
            }
        }

        return Task.FromResult<ProviderCultureResult?>(null);
    }
}
