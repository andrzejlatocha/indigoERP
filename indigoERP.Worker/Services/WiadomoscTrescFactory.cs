using indigoERP.Domain.Entities._Wspolne;

namespace indigoERP.Worker.Services;

public static class WiadomoscTrescFactory
{
    // Wiadomość bez wzorca: temat i treść wprost z formularza.
    public static WiadomoscTresc Prepare(Wiadomosc wiadomosc)
    {
        if (string.IsNullOrWhiteSpace(wiadomosc.wiaTemat))
        {
            throw new InvalidOperationException($"Wiadomość {wiadomosc.wiaID} nie ma tematu.");
        }

        return new WiadomoscTresc(wiadomosc.wiaTemat, wiadomosc.wiaTresc ?? string.Empty);
    }

    public static WiadomoscTresc Prepare(Wiadomosc wiadomosc, WiadomoscWzorzec wzorzec)
    {
        if (string.IsNullOrWhiteSpace(wzorzec.msgTresc))
        {
            throw new InvalidOperationException($"Wzorzec wiadomości {wzorzec.msgID} nie ma treści.");
        }

        var temat = string.IsNullOrWhiteSpace(wiadomosc.wiaTemat) ? wzorzec.msgTemat : wiadomosc.wiaTemat;

        if (string.IsNullOrWhiteSpace(temat))
        {
            throw new InvalidOperationException($"Wiadomość {wiadomosc.wiaID} nie ma tematu.");
        }

        var tresc = wzorzec.msgTresc
            .Replace("{{message}}", wiadomosc.wiaTresc ?? string.Empty, StringComparison.Ordinal)
            .Replace("{{subject}}", temat, StringComparison.Ordinal);

        return new WiadomoscTresc(temat, tresc);
    }
}

public sealed record WiadomoscTresc(string Temat, string Tresc);
