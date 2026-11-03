using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text.RegularExpressions;

namespace indigoERP.Blazor.Pages
{
    // Ogólnodostępna strona (bez logowania) zastępująca link do zewnętrznego
    // Głos Hutnika (wenus.hcz.com.pl/Newspaper.aspx). Świadomie NIE jest komponentem
    // Blazor — cała aplikacja wymaga uwierzytelnienia Windows (Negotiate) na poziomie
    // kręgu Blazor/Hub, więc jedna, prawdziwie anonimowa strona musi ominąć ten krąg
    // i czytać dane bezpośrednio z anonimowych endpointów Api (Wiadomosc/SelectPubliczna,
    // WiadomoscDokument/SelectByWiadomoscPubliczna, WiadomoscDokument/OpenPubliczny,
    // WiadomoscDokument/MiniaturaPubliczna).
    // Przyciski zarządzania (nowy numer, wysyłka) renderuje osadzony komponent Blazor,
    // tylko gdy zalogowany użytkownik ma rolę Gazeta albo Administrator; uprawnienia i tak egzekwuje Api.
    [AllowAnonymous]
    public class WiadomoscPublicznaModel : PageModel
    {
        private const string RolaWiadomosc = "HCZ-indigoERP-Wiadomość";
        private const string RolaAdministrator = "HCZ-indigoERP-Administrator";

        private readonly IHttpClientFactory _httpClientFactory;

        public WiadomoscPublicznaModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<WiadomoscPublicznaViewModel> Wiadomosci { get; private set; } = new();

        public WiadomoscPublicznaViewModel? Wybrana { get; private set; }

        public List<WiadomoscDokumentDto> Zalaczniki { get; private set; } = new();

        // Pierwszy załącznik PDF każdego numeru — z niego powstaje miniatura kafelka.
        public Dictionary<int, WiadomoscDokumentDto> PierwszePdf { get; private set; } = new();

        public bool MozeZarzadzac => User.IsInRole(RolaWiadomosc) || User.IsInRole(RolaAdministrator);

        public async Task OnGetAsync(int? wiaID)
        {
            var client = _httpClientFactory.CreateClient("baseApiClient");

            Wiadomosci = await client.GetFromJsonAsync<List<WiadomoscPublicznaViewModel>>("Wiadomosc/SelectPubliczna") ?? new();

            if (wiaID != null)
            {
                Wybrana = Wiadomosci.FirstOrDefault(w => w.wiaID == wiaID);

                if (Wybrana != null)
                {
                    Zalaczniki = await client.GetFromJsonAsync<List<WiadomoscDokumentDto>>($"WiadomoscDokument/SelectByWiadomoscPubliczna/{wiaID}") ?? new();
                }

                return;
            }

            foreach (var wiadomosc in Wiadomosci)
            {
                var dokumenty = await client.GetFromJsonAsync<List<WiadomoscDokumentDto>>($"WiadomoscDokument/SelectByWiadomoscPubliczna/{wiadomosc.wiaID}");
                var pdf = dokumenty?.FirstOrDefault(d => string.Equals(d.wdoTyp, "pdf", StringComparison.OrdinalIgnoreCase));

                if (pdf != null)
                {
                    PierwszePdf[wiadomosc.wiaID] = pdf;
                }
            }
        }

        // Proxy miniatury przez Blazor host — jak pobieranie, bez wystawiania Api na zewnątrz.
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> OnGetMiniaturaAsync(int wdoID)
        {
            var client = _httpClientFactory.CreateClient("baseApiClient");
            var response = await client.GetAsync($"WiadomoscDokument/MiniaturaPubliczna/{wdoID}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var stream = await response.Content.ReadAsStreamAsync();

            return File(stream, "image/jpeg");
        }

        // Proxy pobierania załącznika przez Blazor host — żeby nie trzeba było
        // wystawiać publicznie bezpośredniego adresu/portu indigoERP.Api.
        public async Task<IActionResult> OnGetDownloadAsync(int wdoID)
        {
            var client = _httpClientFactory.CreateClient("baseApiClient");
            var response = await client.GetAsync($"WiadomoscDokument/OpenPubliczny/{wdoID}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
                ?? response.Content.Headers.ContentDisposition?.FileName?.Trim('"')
                ?? $"dokument-{wdoID}";
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var stream = await response.Content.ReadAsStreamAsync();

            return File(stream, contentType, fileName);
        }

        // Krótki opis na kafelku: treść bez znaczników HTML, ucięta do kilku zdań.
        public static string SkrocOpis(string? trescHtml, int maksDlugosc = 180)
        {
            var tekst = WebUtility.HtmlDecode(Regex.Replace(trescHtml ?? string.Empty, "<[^>]*>", " "));
            tekst = Regex.Replace(tekst, @"\s+", " ").Trim();

            return tekst.Length <= maksDlugosc ? tekst : tekst[..maksDlugosc].TrimEnd() + "…";
        }
    }
}
