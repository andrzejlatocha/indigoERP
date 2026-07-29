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
    public class GrupaService : IGrupaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public GrupaService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<IEnumerable<GrupaViewModel>> Select(string groStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<GrupaViewModel>>($"Grupa/Select?groStatus={groStatus}");

            return result ?? Enumerable.Empty<GrupaViewModel>();
        }

        public async Task<IEnumerable<GrupaRolaViewModel>> SelectRolaByGrupa(int appID, int groID, string srlStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<GrupaRolaViewModel>>($"Grupa/SelectRolaByGrupa/{appID}/{groID}?srlStatus={srlStatus}");

            return result ?? Enumerable.Empty<GrupaRolaViewModel>();
        }

        public async Task<IEnumerable<UzytkownikGrupaViewModel>> SelectUzytkownikByGrupa(int appID, int groID, string usrStatus)
        {
            var httpClient = _httpClientFactory.CreateClient("baseApiClient");
            var url = $"Grupa/SelectUzytkownikByGrupa/{appID}/{groID}?usrStatus={usrStatus}";
            var result = await httpClient.GetFromJsonAsync<IEnumerable<UzytkownikGrupaViewModel>>(url);

            return result ?? Enumerable.Empty<UzytkownikGrupaViewModel>();
        }

        public async Task<GrupaDto> Get(int groID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            return await httpClient.GetFromJsonAsync<GrupaDto>($"Grupa/Get/{groID}");
        }

        public async Task<ResponseMessage> Update(int groID, GrupaDto grupa)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(grupa), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"Grupa/Update/{groID}", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, groID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, groID);
            }
        }

        public async Task<ResponseMessage> Insert(GrupaDto grupa)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(grupa), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("Grupa/Insert", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var newObject = JsonConvert.DeserializeObject<GrupaDto>(apiResponse);

                    return new ResponseMessage((int)response.StatusCode, apiResponse, newObject?.groID);
                }

                return new ResponseMessage(600, apiResponse, null);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        public async Task<ResponseMessage> Delete(int groID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

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

        public async Task<ResponseMessage> InsertCzlonek(GrupaCzlonkowieDto czlonek)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(czlonek), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("Grupa/InsertCzlonek", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var newObject = JsonConvert.DeserializeObject<GrupaCzlonkowieDto>(apiResponse);

                    return new ResponseMessage((int)response.StatusCode, apiResponse, newObject?.uwgID);
                }

                return new ResponseMessage((int)response.StatusCode, apiResponse, null);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        public async Task<ResponseMessage> DeleteCzlonek(int uwgID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                var response = await httpClient.DeleteAsync($"Grupa/DeleteCzlonek/{uwgID}");
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, uwgID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }
    }
}
