using System;
using System.Text.RegularExpressions;

namespace indigoERP.Blazor.OProgramie;

/// <summary>Podstawowe oczyszczanie HTML opisu nowości (opis pochodzi z administracji indigoERP, ale nie ufamy mu bezwarunkowo).</summary>
public static class OProgramieHtml
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(1);

    private static readonly Regex BlockedElements = new(
        @"<\s*(script|style|iframe|object|embed|applet|form|link|meta|base)\b[^>]*>.*?<\s*/\s*\1\s*>",
        RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant, Timeout);

    private static readonly Regex BlockedTags = new(
        @"<\s*/?\s*(script|style|iframe|object|embed|applet|form|link|meta|base)\b[^>]*>",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, Timeout);

    private static readonly Regex EventAttributes = new(
        @"\s+on[a-z]+\s*=\s*(""[^""]*""|'[^']*'|[^\s>]+)",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, Timeout);

    private static readonly Regex DangerousUrls = new(
        @"(?<attr>\s(?:href|src|xlink:href|formaction)\s*=\s*)(?<q>[""']?)\s*(?:javascript|vbscript|data)\s*:[^""'\s>]*\k<q>",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, Timeout);

    /// <summary>Usuwa skrypty, elementy osadzane, atrybuty zdarzeń (onclick…) oraz adresy javascript:/data:.</summary>
    public static string Sanitize(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        try
        {
            var result = BlockedElements.Replace(html, string.Empty);
            result = BlockedTags.Replace(result, string.Empty);
            result = EventAttributes.Replace(result, string.Empty);
            result = DangerousUrls.Replace(result, "${attr}${q}#${q}");

            return result;
        }
        catch (RegexMatchTimeoutException)
        {
            return System.Net.WebUtility.HtmlEncode(html);
        }
    }
}
