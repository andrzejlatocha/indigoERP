using indigoERP.Domain.Entities._Wspolne;
using indigoERP.Worker.Services;

namespace indigoERP.Worker.Tests;

public class WiadomoscTrescFactoryTests
{
    [Fact]
    public void Prepare_ReplacesMessageAndSubjectTokens()
    {
        var wiadomosc = new Wiadomosc { wiaID = 7, wiaTemat = "Temat", wiaTresc = "Treść" };
        var wzorzec = new WiadomoscWzorzec { msgID = 4, msgTemat = "Wzorzec", msgTresc = "<h1>{{subject}}</h1><p>{{message}}</p>" };

        var result = WiadomoscTrescFactory.Prepare(wiadomosc, wzorzec);

        Assert.Equal("Temat", result.Temat);
        Assert.Equal("<h1>Temat</h1><p>Treść</p>", result.Tresc);
    }

    [Fact]
    public void Prepare_UsesTemplateSubjectWhenMessageSubjectIsEmpty()
    {
        var wiadomosc = new Wiadomosc { wiaID = 7, wiaTresc = "Treść" };
        var wzorzec = new WiadomoscWzorzec { msgID = 4, msgTemat = "Wzorzec", msgTresc = "{{message}}" };

        var result = WiadomoscTrescFactory.Prepare(wiadomosc, wzorzec);

        Assert.Equal("Wzorzec", result.Temat);
    }

    [Fact]
    public void Prepare_InsertsHtmlMessageIntoTemplateWithoutEncoding()
    {
        var wiadomosc = new Wiadomosc { wiaID = 12, wiaTemat = "Komunikat", wiaTresc = "<p><strong>Treść HTML</strong></p>" };
        var wzorzec = new WiadomoscWzorzec
        {
            msgID = 6,
            msgTemat = "Wzorzec",
            msgTresc = "<html><body><header>{{subject}}</header><main>{{message}}</main></body></html>"
        };

        var result = WiadomoscTrescFactory.Prepare(wiadomosc, wzorzec);

        Assert.Equal("<html><body><header>Komunikat</header><main><p><strong>Treść HTML</strong></p></main></body></html>", result.Tresc);
    }

    [Fact]
    public void Prepare_ThrowsWhenThereIsNoSubject()
    {
        var wiadomosc = new Wiadomosc { wiaID = 7, wiaTresc = "Treść" };
        var wzorzec = new WiadomoscWzorzec { msgID = 4, msgTresc = "{{message}}" };

        Assert.Throws<InvalidOperationException>(() => WiadomoscTrescFactory.Prepare(wiadomosc, wzorzec));
    }
}
