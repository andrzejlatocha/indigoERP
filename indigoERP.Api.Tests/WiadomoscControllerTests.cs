using indigoERP.Api.Controllers._Wspolne;
using indigoERP.Domain.Dtos._Wspolne;
using Microsoft.AspNetCore.Mvc;

namespace indigoERP.Api.Tests;

public sealed class WiadomoscControllerTests
{
    [Fact]
    public async Task Wiadomosc_insert_maps_template_identifier()
    {
        await using var context = ApiTestContextFactory.Create();
        var controller = new WiadomoscController(context);
        var dto = new WiadomoscDto
        {
            wiaEmail = "nadawca@hcz.com.pl",
            wiaTemat = "Temat",
            wiaTresc = "Treść",
            wiaWzorzecID = 15,
            wiaStatus = "2"
        };

        var result = await controller.Insert(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdDto = Assert.IsType<WiadomoscDto>(created.Value);
        var entity = await context.Wiadomosc.FindAsync(createdDto.wiaID);

        Assert.Equal(15, createdDto.wiaWzorzecID);
        Assert.Equal(15, entity!.wiaWzorzecID);
    }

    [Fact]
    public void Wiadomosc_view_model_matches_current_database_view()
    {
        var properties = typeof(indigoERP.Domain.ViewModels._Wspolne.WiadomoscViewModel)
            .GetProperties()
            .Select(property => property.Name)
            .ToHashSet();

        Assert.Contains("wiaUzytkownikID", properties);
        Assert.Contains("usrImieNazwisko", properties);
        Assert.DoesNotContain("wuzUzytkownikID", properties);
        Assert.DoesNotContain("wuzOdczyt", properties);
        Assert.DoesNotContain("wiaUzytkownikNazwa", properties);
    }
}
