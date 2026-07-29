using indigoERP.Domain.Enumerators._Administrator;
using indigoERP.Service.Extensions;
using indigoERP.Service.Interfaces._Administrator;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace indigoERP.Service.Services._Administrator
{
    public class PracownikService : IPracownikService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public PracownikService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<IEnumerable<PracownikEnumerator>> SelectLookup()
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<PracownikEnumerator>>("Pracownik/SelectLookup");

            return result ?? Enumerable.Empty<PracownikEnumerator>();
        }
    }
}
