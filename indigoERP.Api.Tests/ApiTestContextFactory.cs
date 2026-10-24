using System.Security.Claims;
using indigoERP.Api.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Tests;

internal static class ApiTestContextFactory
{
    public static indigoERPContext Create(int? userId = 101)
    {
        var options = new DbContextOptionsBuilder<indigoERPContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;
        var accessor = new HttpContextAccessor();
        var context = new DefaultHttpContext();

        if (userId.HasValue)
        {
            context.User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim("usrID", userId.Value.ToString())], "Test"));
        }

        accessor.HttpContext = context;
        return new indigoERPContext(options, accessor);
    }

    public static IConfiguration Configuration(int appId = 132) => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?> { ["appSettings:AppID"] = appId.ToString() })
        .Build();
}
