using System.Globalization;
using System.Text;
using API.Constants;
using API.Data;
using API.Models;
using API.Providers;
using API.Services.Implementations;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

builder.Services.AddSingleton<LanguageProvider>();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo(LanguageConstants.Fr),
        new CultureInfo(LanguageConstants.En),
    };

    options.DefaultRequestCulture = new RequestCulture(LanguageConstants.Fr);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    
    options.RequestCultureProviders.Clear();
    options.RequestCultureProviders.Add(new LanguageProvider());
    options.RequestCultureProviders.Add(new QueryStringRequestCultureProvider());
    options.RequestCultureProviders.Add(new CookieRequestCultureProvider());
    options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());
});

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!)),
            ValidateIssuerSigningKey = true
        };
    });




builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddSession();
builder.Services.AddSingleton<ITempDataProvider, SessionStateTempDataProvider>();


builder.Services.Configure<Microsoft.AspNetCore.Mvc.Razor.RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Clear();

    // Recherche par défaut
    options.ViewLocationFormats.Add("/Views/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");

    // Recherche dans le dossier Templates
    options.ViewLocationFormats.Add("/Templates/Emails/{0}.cshtml");
});



builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITranslationService, TranslationService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost4200", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


//@todo décommenter pour la prod.
//app.UseHttpsRedirection();

app.UseRequestLocalization();

app.UseCors("AllowLocalhost4200");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
