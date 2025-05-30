using System;
using System.Globalization;
using API.Services.Interfaces;
using Microsoft.Extensions.Localization;

namespace API.Services.Implementations;

public class TranslationService : ITranslationService
{
    private readonly IStringLocalizer<TranslationService> _localizer;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TranslationService(
        IStringLocalizer<TranslationService> localizer,
        IHttpContextAccessor httpContextAccessor)
    {
        _localizer = localizer;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetTranslation(string key, string? culture = null)
    {
        if (!string.IsNullOrEmpty(culture))
        {
            var previousCulture = CultureInfo.CurrentCulture;
            var previousUICulture = CultureInfo.CurrentUICulture;

            try
            {
                var cultureInfo = new CultureInfo(culture);
                CultureInfo.CurrentCulture = cultureInfo;
                CultureInfo.CurrentUICulture = cultureInfo;

                return _localizer[key].Value;
            }
            finally
            {
                CultureInfo.CurrentCulture = previousCulture;
                CultureInfo.CurrentUICulture = previousUICulture;
            }
        }

        return _localizer[key].Value;
    }

    public string GetCurrentCultureCode()
    {
        return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
    }
}
