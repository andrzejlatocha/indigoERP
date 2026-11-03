using System.Net;
using System.Text.RegularExpressions;

namespace indigoERP.Worker.Services;

public sealed record UndeliveryNotice(DateTimeOffset Received, string Tresc);

public static class NdrDopasowanie
{
    public static bool CzyPowiadomienie(string nadawca, string temat) =>
        nadawca.Contains("mailer-daemon", StringComparison.OrdinalIgnoreCase)
        || nadawca.Contains("postmaster", StringComparison.OrdinalIgnoreCase)
        || temat.StartsWith("Undeliverable", StringComparison.OrdinalIgnoreCase)
        || temat.StartsWith("Undelivered", StringComparison.OrdinalIgnoreCase)
        || temat.StartsWith("Delivery Status Notification", StringComparison.OrdinalIgnoreCase)
        || temat.StartsWith("Nie można dostarczyć", StringComparison.OrdinalIgnoreCase)
        || temat.Contains("Nie dostarczono", StringComparison.OrdinalIgnoreCase);

    public static IReadOnlyList<string> AdresyWTresci(string tresc, IEnumerable<string> adresy) =>
        adresy
            .Where(adres => Regex.IsMatch(
                tresc,
                $@"(?<![\w.+\-]){Regex.Escape(adres)}(?![\w.\-])",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            .ToList();

    public static string OczyscHtml(string html)
    {
        var bezTagow = Regex.Replace(html, "<[^>]+>", " ");
        return Regex.Replace(WebUtility.HtmlDecode(bezTagow), @"\s+", " ").Trim();
    }
}
