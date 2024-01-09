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
    public class RolaService : IRolaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public RolaService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<IEnumerable<Rola>> Select(string srlStatus)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

            try
            {
                var result = await httpClient.GetFromJsonAsync<IEnumerable<Rola>>($"Rola/Select?srlStatus={srlStatus}");

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Rola> Get(int srlID)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

            try
            {
                var result = await httpClient.GetFromJsonAsync<Rola>($"Rola/Get/{srlID}");

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResponseMessage> Update(int srlID, Rola Rola)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(Rola), Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync($"Rola/Update/{srlID}", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, srlID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, srlID);
            }
        }

        public async Task<ResponseMessage> Insert(Rola Rola)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(Rola), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("Rola/Insert", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var newObject = JsonConvert.DeserializeObject<Rola>(apiResponse);

                    return new ResponseMessage((int)response.StatusCode, apiResponse, newObject?.srlID);
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

        public async Task<ResponseMessage> Delete(int srlID)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

            try
            {
                var response = await httpClient.DeleteAsync($"Rola/Delete/{srlID}");
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, srlID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

    }
}
