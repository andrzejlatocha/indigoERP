using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace indigoERP.Blazor.Pages
{
    // Publiczna strona Głosu Hutnika (/gloshutnika): numery z wybranego roku, najnowsze pierwsze.
    // Miniatury i pobieranie PDF idą przez handlery strony WiadomoscPubliczna (anonimowe, bez logowania).
    [AllowAnonymous]
    public class GlosHutnikaModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public GlosHutnikaModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Lata, w których istnieje co najmniej jeden numer — malejąco, do listy wyboru.
        public List<int> Lata { get; private set; } = new();

        public int Rok { get; private set; }

        public List<NumerGlosu> Numery { get; private set; } = new();

        public async Task OnGetAsync(int? rok)
        {
            var client = _httpClientFactory.CreateClient("baseApiClient");
            var wiadomosci = await client.GetFromJsonAsync<List<WiadomoscPublicznaViewModel>>("Wiadomosc/SelectPubliczna") ?? new();

            Lata = wiadomosci
                .Where(w => w.wiaData != null)
                .Select(w => w.wiaData!.Value.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            Rok = rok ?? Lata.FirstOrDefault(DateTime.Now.Year);

            foreach (var wiadomosc in wiadomosci.Where(w => w.wiaData?.Year == Rok).OrderByDescending(w => w.wiaData))
            {
                var dokumenty = await client.GetFromJsonAsync<List<WiadomoscDokumentDto>>(
                    $"WiadomoscDokument/SelectByWiadomoscPubliczna/{wiadomosc.wiaID}");
                var pdf = dokumenty?.FirstOrDefault(d => string.Equals(d.wdoTyp, "pdf", StringComparison.OrdinalIgnoreCase));

                if (pdf != null)
                {
                    Numery.Add(new NumerGlosu(wiadomosc.wiaID, wiadomosc.wiaData, pdf));
                }
            }
        }

        public sealed record NumerGlosu(int WiaID, DateTime? Data, WiadomoscDokumentDto Pdf);
    }
}
