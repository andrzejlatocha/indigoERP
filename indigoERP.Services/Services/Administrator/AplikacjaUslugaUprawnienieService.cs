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
    public class AplikacjaUslugaUprawnienieService : IAplikacjaUslugaUprawnienieService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AplikacjaUslugaUprawnienieService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<IEnumerable<AplikacjaUslugaUprawnienieViewModel>> Select(string usgStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<AplikacjaUslugaUprawnienieViewModel>>($"AplikacjaUslugaUprawnienie/Select?usgStatus={usgStatus}");

            return result ?? Enumerable.Empty<AplikacjaUslugaUprawnienieViewModel>();
        }

        public async Task<AplikacjaUslugaUprawnienieDto> Get(int usgID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            return await httpClient.GetFromJsonAsync<AplikacjaUslugaUprawnienieDto>($"AplikacjaUslugaUprawnienie/Get/{usgID}");
        }

        public async Task<ResponseMessage> Update(int usgID, AplikacjaUslugaUprawnienieDto usluga)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(usluga), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"AplikacjaUslugaUprawnienie/Update/{usgID}", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, usgID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, usgID);
            }
        }

        public async Task<ResponseMessage> Insert(AplikacjaUslugaUprawnienieDto usluga)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(usluga), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("AplikacjaUslugaUprawnienie/Insert", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var newObject = JsonConvert.DeserializeObject<AplikacjaUslugaUprawnienieDto>(apiResponse);

                    return new ResponseMessage((int)response.StatusCode, apiResponse, newObject?.usgID);
                }

                return new ResponseMessage(600, apiResponse, null);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        public async Task<ResponseMessage> Delete(int usgID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                var response = await httpClient.DeleteAsync($"AplikacjaUslugaUprawnienie/Delete/{usgID}");
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, usgID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }
    }
}
