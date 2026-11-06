using indigoERP.Api.Controllers._Slownik;
using indigoERP.Domain.Enumerators._Slownik;

namespace indigoERP.Api.Tests;

public class TypPlikControllerTests
{
    [Fact]
    public async Task SelectLookupByGrupa_ZwracaTypyDlaAplikacjiIGrupyLubBezNich()
    {
        using var context = ApiTestContextFactory.Create();
        context.TypPlikEnumerator.AddRange(
            new TypPlikEnumerator { tpkOpis = "Aplikacja i grupa", tpkAplikacjaID = 900, tpkTypPlikGrupaID = 1, tpkStatus = "2" },
            new TypPlikEnumerator { tpkOpis = "Wspolny dla grupy", tpkAplikacjaID = 0, tpkTypPlikGrupaID = 1, tpkStatus = "2" },
            new TypPlikEnumerator { tpkOpis = "Wspolny dla aplikacji", tpkAplikacjaID = 900, tpkTypPlikGrupaID = 0, tpkStatus = "2" },
            new TypPlikEnumerator { tpkOpis = "Inna aplikacja", tpkAplikacjaID = 100, tpkTypPlikGrupaID = 1, tpkStatus = "2" },
            new TypPlikEnumerator { tpkOpis = "Inna grupa", tpkAplikacjaID = 900, tpkTypPlikGrupaID = 2, tpkStatus = "2" });
        await context.SaveChangesAsync();
        var controller = new TypPlikController(context);

        var result = await controller.SelectLookupByGrupa(900, 1, "2");

        var lista = Assert.IsAssignableFrom<IEnumerable<TypPlikEnumerator>>(result.Value).Select(x => x.tpkOpis).OrderBy(x => x).ToList();
        Assert.Equal(new[] { "Aplikacja i grupa", "Wspolny dla aplikacji", "Wspolny dla grupy" }, lista);
    }

    [Fact]
    public async Task SelectLookupByGrupa_FiltrujeStatus_ZGwiazdkaZwracaWszystkie()
    {
        using var context = ApiTestContextFactory.Create();
        context.TypPlikEnumerator.AddRange(
            new TypPlikEnumerator { tpkOpis = "Aktywny", tpkAplikacjaID = 900, tpkTypPlikGrupaID = 1, tpkStatus = "2" },
            new TypPlikEnumerator { tpkOpis = "Nieaktywny", tpkAplikacjaID = 900, tpkTypPlikGrupaID = 1, tpkStatus = "1" });
        await context.SaveChangesAsync();
        var controller = new TypPlikController(context);

        var aktywne = Assert.IsAssignableFrom<IEnumerable<TypPlikEnumerator>>((await controller.SelectLookupByGrupa(900, 1, "2")).Value).ToList();
        var wszystkie = Assert.IsAssignableFrom<IEnumerable<TypPlikEnumerator>>((await controller.SelectLookupByGrupa(900, 1, "*")).Value).ToList();

        Assert.Equal(new[] { "Aktywny" }, aktywne.Select(x => x.tpkOpis));
        Assert.Equal(2, wszystkie.Count);
    }
}
