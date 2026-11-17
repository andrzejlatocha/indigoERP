using indigoERP.Domain.ViewModels._Wspolne;
using indigoERP.Service.Interfaces._Wspolne;

namespace indigoERP.Blazor.Tests;

/// <summary>Atrapa IPodgladService: zwraca przygotowane powiadomienia i liczy wywołania listy.</summary>
internal sealed class PodgladServiceStub : IPodgladService
{
    public List<PowiadomienieViewModel> Powiadomienia { get; set; } = new();

    /// <summary>Wynik OznaczWszystkiePowiadomienia: liczba albo null (błąd).</summary>
    public int? WynikOznaczWszystkich { get; set; } = 0;

    public bool WynikOznaczPojedynczego { get; set; } = true;

    public int PobraniaListy { get; private set; }

    public Task<IEnumerable<PowiadomienieViewModel>> PowiadomieniaOstatnie(int appID, int usrID, int take = 50)
    {
        PobraniaListy++;
        return Task.FromResult<IEnumerable<PowiadomienieViewModel>>(Powiadomienia);
    }

    public Task<bool> OznaczPowiadomienie(int powID) => Task.FromResult(WynikOznaczPojedynczego);

    public Task<int?> OznaczWszystkiePowiadomienia(int appID, int usrID) => Task.FromResult(WynikOznaczWszystkich);

    public Task<IEnumerable<WiadomoscUzytkownikViewModel>> WiadomosciNieprzeczytane(int appID, int usrID) =>
        Task.FromResult<IEnumerable<WiadomoscUzytkownikViewModel>>(Array.Empty<WiadomoscUzytkownikViewModel>());

    public Task<IEnumerable<ZadanieViewModel>> ZadaniaNieprzeczytane(int appID, int usrID) =>
        Task.FromResult<IEnumerable<ZadanieViewModel>>(Array.Empty<ZadanieViewModel>());

    public Task<IEnumerable<ZdarzenieViewModel>> ZdarzeniaNieprzeczytane(int appID, int usrID) =>
        Task.FromResult<IEnumerable<ZdarzenieViewModel>>(Array.Empty<ZdarzenieViewModel>());

    public static PowiadomienieViewModel Powiadomienie(int powID, string tytul, string? link = null, bool odczytane = false) => new()
    {
        powID = powID,
        powAplikacjaID = 132,
        powUzytkownikID = 7,
        powKodRodzaj = "TEST",
        powTytul = tytul,
        powTresc = "Treść",
        powLink = link,
        powDataPowiadomienia = DateTime.Now,
        powOdczyt = odczytane
    };
}
