using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace indigoERP.Blazor.Pages
{
    // Ogólnodostępna strona (bez logowania) zastępująca link do zewnętrznego
    // Głos Hutnika (wenus.hcz.com.pl/Newspaper.aspx). Świadomie NIE jest komponentem
    // Blazor — cała aplikacja wymaga uwierzytelnienia Windows (Negotiate) na poziomie
    // kręgu Blazor/Hub, więc jedna, prawdziwie anonimowa strona musi ominąć ten krąg
    // i czytać dane bezpośrednio z anonimowych endpointów Api (Wiadomosc/SelectPubliczna,
    // WiadomoscDokument/SelectByWiadomoscPubliczna, WiadomoscDokument/OpenPubliczny).
    [AllowAnonymous]
    public class GlosHutnikaModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public GlosHutnikaModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<WiadomoscPublicznaViewModel> Wiadomosci { get; private set; } = new();

        public WiadomoscPublicznaViewModel? Wybrana { get; private set; }

        public List<WiadomoscDokumentDto> Zalaczniki { get; private set; } = new();

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
            }
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
    }
}
