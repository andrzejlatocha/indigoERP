using indigoERP.Domain.Dtos._Administrator;
using indigoERP.Domain.Enumerators._Administrator;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels;
using indigoERP.Service.Extensions;
using indigoERP.Service.Interfaces._Administrator;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace indigoERP.Service.Services._Administrator
{
    public class AplikacjaService : IAplikacjaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AplikacjaService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<IEnumerable<AplikacjaViewModel>> Select(string appStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<AplikacjaViewModel>>($"Aplikacja/Select?appStatus={appStatus}");

            return result ?? Enumerable.Empty<AplikacjaViewModel>();
        }

        public async Task<IEnumerable<AplikacjaEnumerator>> SelectLookup(string appStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<AplikacjaEnumerator>>($"Aplikacja/SelectLookup?appStatus={appStatus}");

            return result ?? Enumerable.Empty<AplikacjaEnumerator>();
        }

        public async Task<AplikacjaDto> Get(int appID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            return await httpClient.GetFromJsonAsync<AplikacjaDto>($"Aplikacja/Get/{appID}");
        }

        public async Task<ResponseMessage> Update(int appID, AplikacjaDto aplikacja)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(aplikacja), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"Aplikacja/Update/{appID}", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, appID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, appID);
            }
        }

        public async Task<ResponseMessage> Insert(AplikacjaDto aplikacja)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(aplikacja), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("Aplikacja/Insert", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var newObject = JsonConvert.DeserializeObject<AplikacjaDto>(apiResponse);

                    return new ResponseMessage((int)response.StatusCode, apiResponse, newObject?.appID);
                }

                return new ResponseMessage(600, apiResponse, null);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        public async Task<ResponseMessage> Delete(int appID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                var response = await httpClient.DeleteAsync($"Aplikacja/Delete/{appID}");
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, appID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }
    }
}
