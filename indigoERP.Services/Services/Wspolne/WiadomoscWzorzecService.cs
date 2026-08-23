using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Enumerators._Wspolne;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;
using indigoERP.Service.Extensions;
using indigoERP.Service.Interfaces._Wspolne;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace indigoERP.Service.Services._Wspolne
{
    public class WiadomoscWzorzecService : IWiadomoscWzorzecService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public WiadomoscWzorzecService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<IEnumerable<WiadomoscWzorzecViewModel>> Select(int msgAplikacjaID, string msgStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<WiadomoscWzorzecViewModel>>($"WiadomoscWzorzec/Select?msgAplikacjaID={msgAplikacjaID}&msgStatus={msgStatus}");

            return result ?? Enumerable.Empty<WiadomoscWzorzecViewModel>();
        }

        public async Task<IEnumerable<WiadomoscWzorzecEnumerator>> SelectLookup(int msgAplikacjaID, string msgStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<WiadomoscWzorzecEnumerator>>($"WiadomoscWzorzec/SelectLookup?msgAplikacjaID={msgAplikacjaID}&msgStatus={msgStatus}");

            return result ?? Enumerable.Empty<WiadomoscWzorzecEnumerator>();
        }

        public async Task<WiadomoscWzorzecDto> Get(int msgID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            return await httpClient.GetFromJsonAsync<WiadomoscWzorzecDto>($"WiadomoscWzorzec/Get/{msgID}");
        }

        public async Task<ResponseMessage> Update(int msgID, WiadomoscWzorzecDto wzorzec)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(wzorzec), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"WiadomoscWzorzec/Update/{msgID}", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, msgID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, msgID);
            }
        }

        public async Task<ResponseMessage> Insert(WiadomoscWzorzecDto wzorzec)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(wzorzec), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("WiadomoscWzorzec/Insert", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var newObject = JsonConvert.DeserializeObject<WiadomoscWzorzecDto>(apiResponse);

                    return new ResponseMessage((int)response.StatusCode, apiResponse, newObject?.msgID);
                }

                return new ResponseMessage(600, apiResponse, null);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        public async Task<ResponseMessage> Delete(int msgID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                var response = await httpClient.DeleteAsync($"WiadomoscWzorzec/Delete/{msgID}");
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, msgID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }
    }
}
