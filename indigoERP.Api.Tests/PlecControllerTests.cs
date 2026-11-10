using indigoERP.Api.Controllers._Slownik;
using indigoERP.Domain.Enumerators._Slownik;

namespace indigoERP.Api.Tests;

public class PlecControllerTests
{
    [Fact]
    public async Task Select_ZwracaTylkoPozycjeZPodanymStatusem()
    {
        using var context = ApiTestContextFactory.Create();
        context.PlecEnumerator.AddRange(
            new PlecEnumerator { pleKOD = "K", pleOpis = "Kobieta", pleStatus = "2" },
            new PlecEnumerator { pleKOD = "M", pleOpis = "Mężczyzna", pleStatus = "2" },
            new PlecEnumerator { pleKOD = "X", pleOpis = "Wycofana", pleStatus = "1" });
        await context.SaveChangesAsync();
        var controller = new PlecController(context);

        var result = await controller.Select("2");

        var lista = Assert.IsAssignableFrom<IEnumerable<PlecEnumerator>>(result.Value).OrderBy(x => x.pleKOD).ToList();
        Assert.Equal(new[] { "K", "M" }, lista.Select(x => x.pleKOD));
        Assert.Equal(new[] { "Kobieta", "Mężczyzna" }, lista.Select(x => x.pleOpis));
    }

    [Fact]
    public async Task Select_BezPozycjiZTymStatusem_ZwracaPustaListe()
    {
        using var context = ApiTestContextFactory.Create();
        context.PlecEnumerator.Add(new PlecEnumerator { pleKOD = "K", pleOpis = "Kobieta", pleStatus = "2" });
        await context.SaveChangesAsync();
        var controller = new PlecController(context);

        var result = await controller.Select("1");

        Assert.Empty(Assert.IsAssignableFrom<IEnumerable<PlecEnumerator>>(result.Value));
    }
}