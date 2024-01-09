using indigoERP.Domain.Entities._Administrator;
using indigoERP.Domain.Models._Wspolne;
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

        public async Task<IEnumerable<Aplikacja>> Select(string appStatus)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

            try
            {
                var result = await httpClient.GetFromJsonAsync<IEnumerable<Aplikacja>>($"Aplikacja/Select?appStatus={appStatus}");

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Aplikacja> Get(int appID)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

            try
            {
                var result = await httpClient.GetFromJsonAsync<Aplikacja>($"Aplikacja/Get/{appID}");

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResponseMessage> Update(int appID, Aplikacja Aplikacja)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(Aplikacja), Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync($"Aplikacja/Update/{appID}", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, appID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, appID);
            }
        }

        public async Task<ResponseMessage> Insert(Aplikacja Aplikacja)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(Aplikacja), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("Aplikacja/Insert", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var newObject = JsonConvert.DeserializeObject<Aplikacja>(apiResponse);

                    return new ResponseMessage((int)response.StatusCode, apiResponse, newObject?.appID);
                }
                else
                {
                    return new ResponseMessage(600, apiResponse, null);
                }
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        public async Task<ResponseMessage> Delete(int appID)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

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
