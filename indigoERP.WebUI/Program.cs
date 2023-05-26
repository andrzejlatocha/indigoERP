using indigoERP.Service.Interfaces._Autoryzacja;
using indigoERP.Service.Services._Autoryzacja;
using Microsoft.AspNetCore.Authentication.Negotiate;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using indigoERP.Service.Interfaces;
using indigoERP.Service.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;
var connectionString = builder.Configuration.GetConnectionString("indigoERPConnection");

// Add services to the container.
services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});
services.AddRazorPages().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null).AddRazorRuntimeCompilation(); ;

string? baseApiUrl = configuration.GetSection("appSettings").GetValue<string>(key: "BaseApiUrl");
services.AddHttpClient("indigoERPClient", client =>
{
    client.BaseAddress = new Uri(baseApiUrl);
    client.DefaultRequestHeaders.Clear();
});

//Autoryzacja
services.AddSingleton<ILoginService, LoginService>();

//Aplikacja
services.AddSingleton<IUzytkownikAplikacjaService, UzytkownikAplikacjaService>();

services.AddHttpContextAccessor();
services.AddLocalization();

var cultureInfo = new CultureInfo("pl-PL");

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookie";
    options.DefaultChallengeScheme = "Cookie";
})
     .AddCookie("Cookie", options =>
     {
         options.LoginPath = "/_Autoryzacja/Login";
         options.AccessDeniedPath = "/Denied";
         options.ExpireTimeSpan = TimeSpan.FromMinutes(configuration.GetSection("appSettings").GetValue<int>(key: "timeOut"));
     });

services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error/");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();
