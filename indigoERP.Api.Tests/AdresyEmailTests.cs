using indigoERP.Domain.Common;
using Xunit;

namespace indigoERP.Api.Tests;

public class AdresyEmailTests
{
    [Theory]
    [InlineData("jan.kowalski@huta-czestochowa.pl")]
    [InlineData("a@b.pl")]
    [InlineData("imie+tag@firma.com.pl")]
    public void CzyPoprawny_akceptuje_poprawne_adresy(string adres)
    {
        Assert.True(AdresyEmail.CzyPoprawny(adres));
    }

    [Theory]
    [InlineData("")]
    [InlineData("bez-malpy.pl")]
    [InlineData("dwa@@znaki.pl")]
    [InlineData("spacja w@adresie.pl")]
    [InlineData("brak@kropki")]
    [InlineData("@brak-lokalnej.pl")]
    public void CzyPoprawny_odrzuca_bledne_adresy(string adres)
    {
        Assert.False(AdresyEmail.CzyPoprawny(adres));
    }

    [Fact]
    public void Waliduj_przycina_pomija_puste_i_usuwa_duplikaty_bez_wzgledu_na_wielkosc_liter()
    {
        var wynik = AdresyEmail.Waliduj(["  jan@firma.pl ", "", "JAN@firma.pl", "anna@firma.pl"]);

        Assert.Null(wynik.Blad);
        Assert.Equal(["jan@firma.pl", "anna@firma.pl"], wynik.Adresy);
    }

    [Fact]
    public void Waliduj_zwraca_blad_dla_pustej_listy()
    {
        var wynik = AdresyEmail.Waliduj(["", "   "]);

        Assert.Empty(wynik.Adresy);
        Assert.NotNull(wynik.Blad);
    }

    [Fact]
    public void Waliduj_zwraca_blad_wskazujacy_pierwszy_nieprawidlowy_adres()
    {
        var wynik = AdresyEmail.Waliduj(["ok@firma.pl", "zly-adres", "tez@zly"]);

        Assert.Empty(wynik.Adresy);
        Assert.Contains("zly-adres", wynik.Blad);
    }

    [Fact]
    public void Waliduj_odrzuca_zbyt_duza_liczbe_adresow()
    {
        var adresy = Enumerable.Range(1, AdresyEmail.MaksymalnaLiczba + 1).Select(i => $"u{i}@firma.pl").ToList();

        var wynik = AdresyEmail.Waliduj(adresy);

        Assert.NotNull(wynik.Blad);
    }
}
