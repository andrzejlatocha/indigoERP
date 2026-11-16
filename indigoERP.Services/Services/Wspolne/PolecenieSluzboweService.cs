using indigoERP.Domain.ViewModels._Wspolne;
using indigoERP.Service.Extensions;
using indigoERP.Service.Interfaces._Wspolne;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using System.Net.Http.Json;

namespace indigoERP.Service.Services._Wspolne
{
    public class PolecenieSluzboweService : IPolecenieSluzboweService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public PolecenieSluzboweService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<IEnumerable<PrzepisWewnetrznyViewModel>> SelectPubliczne(string? szukaj, int? katalogID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var parametry = new List<string>();

            if (!string.IsNullOrWhiteSpace(szukaj))
            {
                parametry.Add("szukaj=" + Uri.EscapeDataString(szukaj.Trim()));
            }

            if (katalogID is int wybrany)
            {
                parametry.Add("katalogID=" + wybrany);
            }

            var sciezka = "PrzepisyWewnetrzne/SelectPubliczne" + (parametry.Count > 0 ? "?" + string.Join("&", parametry) : string.Empty);

            var result = await httpClient.GetFromJsonAsync<List<PrzepisWewnetrznyViewModel>>(sciezka);

            return result ?? new List<PrzepisWewnetrznyViewModel>();
        }

        public async Task<IEnumerable<KatalogWyborViewModel>> SelectKatalogi()
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<List<KatalogWyborViewModel>>("PrzepisyWewnetrzne/SelectKatalogi");

            return result ?? new List<KatalogWyborViewModel>();
        }

        public async Task<byte[]?> Pdf(int pliID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            using var odpowiedz = await httpClient.GetAsync($"PrzepisyWewnetrzne/Pdf/{pliID}");

            if (odpowiedz.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            odpowiedz.EnsureSuccessStatusCode();

            return await odpowiedz.Content.ReadAsByteArrayAsync();
        }
    }
}
