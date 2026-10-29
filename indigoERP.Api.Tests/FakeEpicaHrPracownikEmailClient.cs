using indigoERP.Api.Services;

namespace indigoERP.Api.Tests;

/// <summary>Atrapa klienta epicaHR: rejestruje wywołania; Blad != null symuluje odrzucenie przez epicaHR.</summary>
internal sealed class FakeEpicaHrPracownikEmailClient : IEpicaHrPracownikEmailClient
{
    public List<(int PracownikID, string? Email)> Wywolania { get; } = new();

    public string? Blad { get; set; }

    public Task<string?> UstawEmailSluzbowyAsync(int pracownikID, string? email, CancellationToken cancellationToken)
    {
        Wywolania.Add((pracownikID, email));
        return Task.FromResult(Blad);
    }
}
