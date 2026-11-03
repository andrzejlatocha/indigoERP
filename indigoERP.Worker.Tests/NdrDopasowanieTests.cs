using indigoERP.Worker.Services;

namespace indigoERP.Worker.Tests;

public class NdrDopasowanieTests
{
    [Theory]
    [InlineData("MAILER-DAEMON@huta-czestochowa.pl", "Cokolwiek")]
    [InlineData("postmaster@huta-czestochowa.pl", "Cokolwiek")]
    [InlineData("ktos@huta-czestochowa.pl", "Undeliverable: Test")]
    [InlineData("ktos@huta-czestochowa.pl", "Nie można dostarczyć: Test")]
    [InlineData("ktos@huta-czestochowa.pl", "Delivery Status Notification (Failure)")]
    public void CzyPowiadomienie_RozpoznajeOdpowiedziONiedostarczeniu(string nadawca, string temat)
    {
        Assert.True(NdrDopasowanie.CzyPowiadomienie(nadawca, temat));
    }

    [Fact]
    public void CzyPowiadomienie_IgnorujeZwykleWiadomosci()
    {
        Assert.False(NdrDopasowanie.CzyPowiadomienie("jan.kowalski@huta-czestochowa.pl", "Spotkanie w czwartek"));
    }

    [Fact]
    public void AdresyWTresci_ZnajdujeAdresBezRozroznieniaWielkosciLiter()
    {
        var wynik = NdrDopasowanie.AdresyWTresci(
            "Nie dostarczono do: Jan.Kowalski@Huta-Czestochowa.pl",
            ["jan.kowalski@huta-czestochowa.pl", "anna@huta-czestochowa.pl"]);

        Assert.Equal(["jan.kowalski@huta-czestochowa.pl"], wynik);
    }

    [Fact]
    public void AdresyWTresci_NieDopasowujeAdresuBedacegoCzescia_Innego()
    {
        var wynik = NdrDopasowanie.AdresyWTresci(
            "Adres: xjan@huta-czestochowa.pl",
            ["jan@huta-czestochowa.pl"]);

        Assert.Empty(wynik);
    }

    [Fact]
    public void OczyscHtml_UsuwaTagiIDekodujeEncje()
    {
        var wynik = NdrDopasowanie.OczyscHtml("<p>Nie&nbsp;dostarczono:</p><b>jan@huta-czestochowa.pl</b>");

        Assert.Equal("Nie dostarczono: jan@huta-czestochowa.pl", wynik);
    }
}
