using indigoERP.Api.Controllers;
using indigoERP.Api.Services;
using indigoERP.Domain.ViewModels._Wspolne;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace indigoERP.Api.Tests;

public class PrzepisyWewnetrzneControllerTests
{
    [Fact]
    public void Kontroler_wymaga_zalogowania()
    {
        var atrybuty = typeof(PrzepisyWewnetrzneController).GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

        Assert.NotEmpty(atrybuty);
    }

    [Fact]
    public async Task SelectPubliczne_zwraca_liste_przepisow_z_atacamaDMS()
    {
        var klient = new AtacamaPolecenieClientStub
        {
            Lista = [new PrzepisWewnetrznyViewModel { pliID = 1, pliNazwa = "przepis.pdf", pliOpis = "Test" }]
        };
        var controller = new PrzepisyWewnetrzneController(klient);

        var wynik = await controller.SelectPubliczne("test");

        var lista = Assert.IsAssignableFrom<IEnumerable<PrzepisWewnetrznyViewModel>>(Assert.IsType<OkObjectResult>(wynik.Result).Value);
        Assert.Single(lista);
        Assert.Equal("test", klient.OstatniSzukaj);
    }

    [Fact]
    public async Task SelectPubliczne_bez_tokenu_zwraca_503()
    {
        var controller = new PrzepisyWewnetrzneController(new AtacamaPolecenieClientStub { BezTokenu = true });

        var wynik = await controller.SelectPubliczne();

        Assert.Equal(StatusCodes.Status503ServiceUnavailable, Assert.IsType<ObjectResult>(wynik.Result).StatusCode);
    }

    [Fact]
    public async Task SelectPubliczne_blad_atacamaDMS_zwraca_502()
    {
        var controller = new PrzepisyWewnetrzneController(new AtacamaPolecenieClientStub { Blad = true });

        var wynik = await controller.SelectPubliczne();

        Assert.Equal(StatusCodes.Status502BadGateway, Assert.IsType<ObjectResult>(wynik.Result).StatusCode);
    }

    [Fact]
    public async Task Pdf_zwraca_plik_pdf()
    {
        var controller = new PrzepisyWewnetrzneController(new AtacamaPolecenieClientStub { Pdf = [1, 2, 3] });

        var wynik = await controller.Pdf(5);

        var plik = Assert.IsType<FileContentResult>(wynik);
        Assert.Equal("application/pdf", plik.ContentType);
        Assert.Equal(new byte[] { 1, 2, 3 }, plik.FileContents);
    }

    [Fact]
    public async Task Pdf_brak_dokumentu_zwraca_404()
    {
        var controller = new PrzepisyWewnetrzneController(new AtacamaPolecenieClientStub());

        var wynik = await controller.Pdf(5);

        Assert.IsType<NotFoundResult>(wynik);
    }

    private sealed class AtacamaPolecenieClientStub : IAtacamaPolecenieClient
    {
        public IReadOnlyList<PrzepisWewnetrznyViewModel> Lista { get; set; } = [];
        public byte[]? Pdf { get; set; }
        public bool BezTokenu { get; set; }
        public bool Blad { get; set; }
        public string? OstatniSzukaj { get; private set; }

        public Task<IReadOnlyList<PrzepisWewnetrznyViewModel>?> SelectPubliczneAsync(string? szukaj, CancellationToken cancellationToken)
        {
            if (Blad) throw new HttpRequestException("test");
            if (BezTokenu) return Task.FromResult<IReadOnlyList<PrzepisWewnetrznyViewModel>?>(null);

            OstatniSzukaj = szukaj;
            return Task.FromResult<IReadOnlyList<PrzepisWewnetrznyViewModel>?>(Lista);
        }

        public Task<byte[]?> PobierzPdfAsync(int pliID, CancellationToken cancellationToken) => Task.FromResult(Pdf);
    }
}
