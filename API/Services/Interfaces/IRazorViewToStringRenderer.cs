using System;

namespace API.Services.Interfaces;

public interface IRazorViewToStringRenderer
{
    public Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
}
