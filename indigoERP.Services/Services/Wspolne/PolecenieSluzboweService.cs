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

        public async Task<IEnumerable<PrzepisWewnetrznyViewModel>> SelectPubliczne(string? szukaj)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var sciezka = "PrzepisyWewnetrzne/SelectPubliczne";

            if (!string.IsNullOrWhiteSpace(szukaj))
            {
                sciezka += "?szukaj=" + Uri.EscapeDataString(szukaj.Trim());
            }

            var result = await httpClient.GetFromJsonAsync<List<PrzepisWewnetrznyViewModel>>(sciezka);

            return result ?? new List<PrzepisWewnetrznyViewModel>();
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
