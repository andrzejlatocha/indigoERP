using indigoERP.Blazor.Data;
using indigoERP.Blazor.Utils;
using indigoERP.Service.Interfaces._Autoryzacja;
using indigoERP.Service.Services._Autoryzacja;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.
services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

string? baseApiUrl = configuration.GetSection("appSettings").GetValue<string>(key: "BaseApiUrl");
services.AddHttpClient("baseClient", client =>
{
    client.BaseAddress = new Uri(baseApiUrl);
    client.DefaultRequestHeaders.Clear();
});

string? indigoApiUrl = configuration.GetSection("appSettings").GetValue<string>(key: "IndigoApiUrl");
services.AddHttpClient("indigoERPClient", client =>
{
    client.BaseAddress = new Uri(indigoApiUrl);
    client.DefaultRequestHeaders.Clear();
});

// Radzen
services.AddScoped<DialogService>();
services.AddScoped<NotificationService>();
services.AddScoped<ContextMenuService>();
services.AddScoped<TooltipService>();

// Autoryzacja
services.AddSingleton<ILoginService, LoginService>();
services.AddSingleton<IClaimsTransformation, ClaimsTransformation>();

//Aplikacja

services.AddHttpContextAccessor();
services.AddLocalization();

var cultureInfo = new CultureInfo("pl-PL");

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
