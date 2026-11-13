using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using indigoERP.Api.Services;
using Microsoft.IdentityModel.Tokens;

namespace indigoERP.Api.Tests;

public sealed class JwtSigningKeysTests
{
    private const string JwtKey = "indigo-service-key-0123456789abcdef";
    private const string GatewayKey = "authorizationSRV-gateway-key-0123456789";
    private const string UnknownKey = "some-other-signing-key-0123456789";

    [Fact]
    public void Token_signed_with_gateway_key_is_accepted_when_gateway_key_is_configured()
    {
        var token = CreateToken(GatewayKey);

        Assert.True(TryValidate(token, JwtSigningKeys.Create(JwtKey, GatewayKey)));
    }

    [Fact]
    public void Token_signed_with_jwt_key_is_still_accepted()
    {
        var token = CreateToken(JwtKey);

        Assert.True(TryValidate(token, JwtSigningKeys.Create(JwtKey, GatewayKey)));
    }

    [Fact]
    public void Token_signed_with_gateway_key_is_rejected_when_gateway_key_is_not_configured()
    {
        // Bez GatewayKey zachowanie jak przed zmianą: akceptowany jest wyłącznie Jwt:Key.
        var token = CreateToken(GatewayKey);

        Assert.False(TryValidate(token, JwtSigningKeys.Create(JwtKey, null)));
    }

    [Fact]
    public void Token_signed_with_unknown_key_is_rejected()
    {
        var token = CreateToken(UnknownKey);

        Assert.False(TryValidate(token, JwtSigningKeys.Create(JwtKey, GatewayKey)));
    }

    [Fact]
    public void Missing_jwt_key_is_reported_as_configuration_error()
    {
        Assert.Throws<InvalidOperationException>(() => JwtSigningKeys.Create(null, GatewayKey));
    }

    private static string CreateToken(string signingKey)
    {
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: "authorizationSRVAuthenticationServer",
            audience: "authorizationSRVServiceClient",
            claims: [new Claim("usrID", "3105")],
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static bool TryValidate(string token, IEnumerable<SecurityKey> keys)
    {
        // Te same ustawienia co w Program.cs (JwtBearer): podpis i ważność, bez sprawdzania wystawcy i odbiorcy.
        var parameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = keys
        };

        try
        {
            new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _);

            return true;
        }
        catch (SecurityTokenException)
        {
            return false;
        }
    }
}
