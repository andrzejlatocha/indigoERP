using indigoERP.Domain.Enumerators._Slownik;
using indigoERP.Service.Extensions;
using indigoERP.Service.Interfaces._Slownik;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace indigoERP.Service.Services._Slownik
{
    public class SerwerTypService : ISerwerTypService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public SerwerTypService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<IEnumerable<SerwerTypEnumerator>> SelectLookup(string styStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<SerwerTypEnumerator>>($"SerwerTyp/SelectLookup?styStatus={styStatus}");

            return result ?? Enumerable.Empty<SerwerTypEnumerator>();
        }
    }
}
