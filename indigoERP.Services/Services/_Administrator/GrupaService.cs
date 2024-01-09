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
    public class GrupaService : IGrupaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public GrupaService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<IEnumerable<Grupa>> Select(string groStatus)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

            try
            {
                var result = await httpClient.GetFromJsonAsync<IEnumerable<Grupa>>($"Grupa/Select?groStatus={groStatus}");

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Grupa> Get(int groID)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

            try
            {
                var result = await httpClient.GetFromJsonAsync<Grupa>($"Grupa/Get/{groID}");

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResponseMessage> Update(int groID, Grupa Grupa)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(Grupa), Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync($"Grupa/Update/{groID}", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, groID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, groID);
            }
        }

        public async Task<ResponseMessage> Insert(Grupa Grupa)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(Grupa), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("Grupa/Insert", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var newObject = JsonConvert.DeserializeObject<Grupa>(apiResponse);

                    return new ResponseMessage((int)response.StatusCode, apiResponse, newObject?.groID);
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

        public async Task<ResponseMessage> Delete(int groID)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

            try
            {
                var response = await httpClient.DeleteAsync($"Grupa/Delete/{groID}");
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, groID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

    }
}
