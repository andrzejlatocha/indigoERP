using indigoERP.Domain.Models._Wspolne;
using indigoERP.Service.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Text;
using indigoERP.Domain.Entities._Administrator;
using indigoERP.Service.Interfaces._Administrator;

namespace indigoERP.Service.Services._Administrator
{
    public class UzytkownikService : IUzytkownikService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public UzytkownikService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<IEnumerable<Uzytkownik>> Select(string usrStatus)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

            try
            {
                var result = await httpClient.GetFromJsonAsync<IEnumerable<Uzytkownik>>($"Uzytkownik/Select?usrStatus={usrStatus}");

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Uzytkownik> Get(int usrID)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

            try
            {
                var result = await httpClient.GetFromJsonAsync<Uzytkownik>($"Uzytkownik/Get/{usrID}");

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResponseMessage> Update(int usrID, Uzytkownik Uzytkownik)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(Uzytkownik), Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync($"Uzytkownik/Update/{usrID}", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, usrID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, usrID);
            }
        }

        public async Task<ResponseMessage> Insert(Uzytkownik Uzytkownik)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(Uzytkownik), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("Uzytkownik/Insert", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var newObject = JsonConvert.DeserializeObject<Uzytkownik>(apiResponse);

                    return new ResponseMessage((int)response.StatusCode, apiResponse, newObject?.usrID);
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

        public async Task<ResponseMessage> Delete(int usrID)
        {
            var httpClient = new HttpClient().CreateClientWithToken("baseClient", _httpClientFactory, _authenticationStateProvider);

            try
            {
                var response = await httpClient.DeleteAsync($"Uzytkownik/Delete/{usrID}");
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, usrID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }
    }
}
