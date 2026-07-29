using indigoERP.Domain.Dtos._Administrator;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Administracja;
using indigoERP.Domain.ViewModels._Autoryzacja;
using indigoERP.Service.Extensions;
using indigoERP.Service.Interfaces._Administrator;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

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

        public async Task<IEnumerable<UzytkownikViewModel>> Select(string usrStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<UzytkownikViewModel>>($"Uzytkownik/Select?usrStatus={usrStatus}");

            return result ?? Enumerable.Empty<UzytkownikViewModel>();
        }

        public async Task<IEnumerable<UzytkownikGrupaViewModel>> SelectGrupaByUzytkownik(int appID, int usrID, string groStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var url = $"Uzytkownik/SelectGrupaByUzytkownik/{appID}/{usrID}?groStatus={groStatus}";
            var result = await httpClient.GetFromJsonAsync<IEnumerable<UzytkownikGrupaViewModel>>(url);

            return result ?? Enumerable.Empty<UzytkownikGrupaViewModel>();
        }

        public async Task<IEnumerable<UzytkownikRolaViewModel>> SelectRolaByUzytkownik(int appID, int usrID, string srlStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var url = $"Uzytkownik/SelectRolaByUzytkownik/{appID}/{usrID}?srlStatus={srlStatus}";
            var result = await httpClient.GetFromJsonAsync<IEnumerable<UzytkownikRolaViewModel>>(url);

            return result ?? Enumerable.Empty<UzytkownikRolaViewModel>();
        }

        public async Task<UzytkownikDto> Get(int usrID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            return await httpClient.GetFromJsonAsync<UzytkownikDto>($"Uzytkownik/Get/{usrID}");
        }

        public async Task<ResponseMessage> Update(int usrID, UzytkownikDto uzytkownik)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(uzytkownik), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"Uzytkownik/Update/{usrID}", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, usrID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, usrID);
            }
        }

        public async Task<ResponseMessage> Insert(UzytkownikDto uzytkownik)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(uzytkownik), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("Uzytkownik/Insert", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var newObject = JsonConvert.DeserializeObject<UzytkownikDto>(apiResponse);

                    return new ResponseMessage((int)response.StatusCode, apiResponse, newObject?.usrID);
                }

                return new ResponseMessage(600, apiResponse, null);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        public async Task<ResponseMessage> Delete(int usrID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

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
