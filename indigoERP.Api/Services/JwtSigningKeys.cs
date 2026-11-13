using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace indigoERP.Api.Services;

// Klucze, którymi indigoERP.Api sprawdza podpis tokenów JWT.
// Tokeny użytkowników wystawia authorizationSRV i podpisuje je wspólnym sekretem GatewayKey,
// a tokeny usług indigoERP podpisane są kluczem Jwt:Key (zmienna indigoERP_JwtKey).
// Akceptujemy oba klucze. GatewayKey jest opcjonalny: gdy go brak, walidacja działa jak dotąd, tylko z Jwt:Key.
public static class JwtSigningKeys
{
    public static IEnumerable<SecurityKey> Create(string? jwtKey, string? gatewayKey)
    {
        var keys = new List<SecurityKey>
        {
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? throw new InvalidOperationException("Brak wymaganej konfiguracji Jwt:Key.")))
        };

        if (!string.IsNullOrWhiteSpace(gatewayKey))
        {
            keys.Add(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(gatewayKey)));
        }

        return keys;
    }
}
