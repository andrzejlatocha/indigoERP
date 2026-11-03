using System;

namespace indigoERP.Blazor.OProgramie;

/// <summary>Karta cechy systemu: nazwa ikony (shield, news, support, route, docs, gear, chart, info), tytuł i opis.</summary>
public sealed record OProgramieCecha(string Ikona, string Tytul, string Tekst);

/// <summary>Nowość z API indigoERP (Nowosc/SelectByAplikacja) wyświetlana na stronie O programie.</summary>
public sealed class OProgramieNowosc
{
    public int nowID { get; set; }

    public DateTime? nowData { get; set; }

    public int nowAplikacjaID { get; set; }

    public string? nowURL { get; set; }

    public string? nowWersja { get; set; }

    public string? nowTytul { get; set; }

    /// <summary>Pełny opis w formacie HTML (przed wyświetleniem przechodzi przez <see cref="OProgramieHtml.Sanitize" />).</summary>
    public string? nowOpis { get; set; }

    public string? nowTypNowoscKOD { get; set; }

    public string? tnoOpis { get; set; }

    /// <summary>Pełny znacznik ikony (np. &lt;i class="fa-solid fa-circle-info"&gt;&lt;/i&gt;) lub sama klasa CSS.</summary>
    public string? tnoIkona { get; set; }
}
