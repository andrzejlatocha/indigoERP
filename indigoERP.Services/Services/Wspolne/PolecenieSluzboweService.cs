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

        public async Task<IEnumerable<PolecenieSluzboweViewModel>> SelectPubliczne(string? szukaj)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var sciezka = "PolecenieSluzbowe/SelectPubliczne";

            if (!string.IsNullOrWhiteSpace(szukaj))
            {
                sciezka += "?szukaj=" + Uri.EscapeDataString(szukaj.Trim());
            }

            var result = await httpClient.GetFromJsonAsync<List<PolecenieSluzboweViewModel>>(sciezka);

            return result ?? new List<PolecenieSluzboweViewModel>();
        }

        public async Task<byte[]?> Pdf(int plsID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            using var odpowiedz = await httpClient.GetAsync($"PolecenieSluzbowe/Pdf/{plsID}");

            if (odpowiedz.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            odpowiedz.EnsureSuccessStatusCode();

            return await odpowiedz.Content.ReadAsByteArrayAsync();
        }
    }
}
