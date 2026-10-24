using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace indigoERP.Blazor.Tests;

internal sealed class TestAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AuthenticationState authenticationState;

    public TestAuthenticationStateProvider(IEnumerable<Claim>? claims = null)
    {
        var identity = new ClaimsIdentity(claims ?? [], "TestAuthentication");
        authenticationState = new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(authenticationState);
}

internal sealed class TestWebHostEnvironment : IWebHostEnvironment
{
    public string ApplicationName { get; set; } = "indigoERP.Blazor.Tests";
    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    public string WebRootPath { get; set; } = Path.GetTempPath();
    public string EnvironmentName { get; set; } = "Testing";
    public string ContentRootPath { get; set; } = Path.GetTempPath();
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
}
