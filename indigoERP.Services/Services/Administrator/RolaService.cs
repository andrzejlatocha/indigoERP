using indigoERP.Domain.Dtos._Administrator;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Administracja;
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

        public async Task<IEnumerable<RolaViewModel>> Select(string srlStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<RolaViewModel>>($"Rola/Select?srlStatus={srlStatus}");

            return result ?? Enumerable.Empty<RolaViewModel>();
        }

        public async Task<IEnumerable<GrupaRolaViewModel>> SelectGrupaByRola(int appID, int srlID, string groStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<GrupaRolaViewModel>>($"Rola/SelectGrupaByRola/{appID}/{srlID}?groStatus={groStatus}");

            return result ?? Enumerable.Empty<GrupaRolaViewModel>();
        }

        public async Task<IEnumerable<UzytkownikRolaViewModel>> SelectUzytkownikByRola(int appID, int srlID, string usrStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var url = $"Rola/SelectUzytkownikByRola/{appID}/{srlID}?usrStatus={usrStatus}";
            var result = await httpClient.GetFromJsonAsync<IEnumerable<UzytkownikRolaViewModel>>(url);

            return result ?? Enumerable.Empty<UzytkownikRolaViewModel>();
        }

        public async Task<RolaDto> Get(int srlID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            return await httpClient.GetFromJsonAsync<RolaDto>($"Rola/Get/{srlID}");
        }

        public async Task<ResponseMessage> Update(int srlID, RolaDto rola)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(rola), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"Rola/Update/{srlID}", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, srlID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, srlID);
            }
        }

        public async Task<ResponseMessage> Insert(RolaDto rola)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(rola), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("Rola/Insert", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var newObject = JsonConvert.DeserializeObject<RolaDto>(apiResponse);

                    return new ResponseMessage((int)response.StatusCode, apiResponse, newObject?.srlID);
                }

                return new ResponseMessage(600, apiResponse, null);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        public async Task<ResponseMessage> Delete(int srlID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

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

        public async Task<ResponseMessage> InsertCzlonek(RolaCzlonkowieDto czlonek)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(czlonek), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("Rola/InsertCzlonek", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var newObject = JsonConvert.DeserializeObject<RolaCzlonkowieDto>(apiResponse);

                    return new ResponseMessage((int)response.StatusCode, apiResponse, newObject?.cwrID);
                }

                return new ResponseMessage((int)response.StatusCode, apiResponse, null);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        public async Task<ResponseMessage> DeleteCzlonek(int cwrID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                var response = await httpClient.DeleteAsync($"Rola/DeleteCzlonek/{cwrID}");
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, cwrID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }
    }
}
