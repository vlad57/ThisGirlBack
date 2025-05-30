using System;

namespace API.Services.Interfaces;

public interface ITranslationService
{
    string GetTranslation(string key, string? culture = null);
    string GetCurrentCultureCode();
}
