using indigoERP.Domain.Common;
using Xunit;

namespace indigoERP.Api.Tests;

public class HarmonogramTerminyTests
{
    private static readonly DateTime Teraz = new(2027, 3, 10, 9, 0, 0);

    [Fact]
    public void Waliduj_odrzuca_nieznany_rodzaj_cyklu()
    {
        var blad = HarmonogramTerminy.Waliduj("X", Teraz.AddDays(1), null, Teraz);

        Assert.Contains("rodzaj", blad);
    }

    [Fact]
    public void Waliduj_wymaga_terminu_pierwszej_wysylki()
    {
        var blad = HarmonogramTerminy.Waliduj("D", null, null, Teraz);

        Assert.Contains("Podaj termin", blad);
    }

    [Fact]
    public void Waliduj_odrzuca_termin_rowny_teraz_i_wczesniejszy()
    {
        Assert.NotNull(HarmonogramTerminy.Waliduj("J", Teraz, null, Teraz));
        Assert.NotNull(HarmonogramTerminy.Waliduj("J", Teraz.AddMinutes(-1), null, Teraz));
    }

    [Fact]
    public void Waliduj_akceptuje_termin_w_przyszlosci_dla_jednorazowej_wysylki()
    {
        Assert.Null(HarmonogramTerminy.Waliduj("J", Teraz.AddMinutes(1), null, Teraz));
    }

    [Fact]
    public void Waliduj_odrzuca_date_koncowa_wczesniejsza_niz_pierwszy_termin()
    {
        var start = Teraz.AddDays(2);

        var blad = HarmonogramTerminy.Waliduj("D", start, start.AddDays(-1), Teraz);

        Assert.Contains("Data końcowa", blad);
    }

    [Fact]
    public void Waliduj_akceptuje_date_koncowa_rowna_pierwszemu_terminowi()
    {
        var start = Teraz.AddDays(2);

        Assert.Null(HarmonogramTerminy.Waliduj("T", start, start, Teraz));
    }

    [Fact]
    public void Waliduj_ignoruje_date_koncowa_dla_jednorazowej_wysylki()
    {
        var start = Teraz.AddDays(2);

        Assert.Null(HarmonogramTerminy.Waliduj("J", start, start.AddDays(-1), Teraz));
    }

    [Fact]
    public void NastepnyTermin_dodaje_dzien_tydzien_i_miesiac()
    {
        var termin = new DateTime(2027, 3, 10, 8, 30, 0);

        Assert.Equal(new DateTime(2027, 3, 11, 8, 30, 0), HarmonogramTerminy.NastepnyTermin("D", termin));
        Assert.Equal(new DateTime(2027, 3, 17, 8, 30, 0), HarmonogramTerminy.NastepnyTermin("T", termin));
        Assert.Equal(new DateTime(2027, 4, 10, 8, 30, 0), HarmonogramTerminy.NastepnyTermin("M", termin));
    }

    [Fact]
    public void NastepnyTermin_miesieczny_konczy_sie_na_ostatnim_dniu_krotszego_miesiaca()
    {
        // 31 stycznia + miesiąc = 28 lutego (2027 nie jest rokiem przestępnym).
        Assert.Equal(new DateTime(2027, 2, 28), HarmonogramTerminy.NastepnyTermin("M", new DateTime(2027, 1, 31)));
    }

    [Fact]
    public void NastepnyTermin_dla_jednorazowej_wysylki_nie_ma_kolejnego_terminu()
    {
        Assert.Equal(DateTime.MaxValue, HarmonogramTerminy.NastepnyTermin("J", Teraz));
    }
}
