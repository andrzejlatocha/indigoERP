using indigoERP.Domain.Entities._Autoryzacja;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Service.Extensions;
using indigoERP.Service.Interfaces._Administrator;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace indigoERP.Service.Services._Administrator
{
    public class LoginBlokadaService : ILoginBlokadaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public LoginBlokadaService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<IEnumerable<LoginBlokada>> Select(string lgbStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<LoginBlokada>>($"LoginBlokada/Select?lgbStatus={lgbStatus}");

            return result ?? Enumerable.Empty<LoginBlokada>();
        }

        public async Task<ResponseMessage> Odblokuj(int lgbID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                var response = await httpClient.PostAsync($"LoginBlokada/Odblokuj/{lgbID}", null);
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, lgbID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }
    }
}
