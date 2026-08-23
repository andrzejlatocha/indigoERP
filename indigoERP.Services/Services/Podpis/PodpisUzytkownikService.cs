using indigoERP.Domain.Dtos._Podpis;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Podpis;
using indigoERP.Service.Extensions;
using indigoERP.Service.Interfaces._Podpis;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace indigoERP.Service.Services._Podpis
{
    public class PodpisUzytkownikService : IPodpisUzytkownikService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public PodpisUzytkownikService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<IEnumerable<PodpisUzytkownikViewModel>> SelectByPodpis(int pdpID, string pduStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<PodpisUzytkownikViewModel>>($"PodpisUzytkownik/SelectByPodpis/{pdpID}?pduStatus={pduStatus}");

            return result ?? Enumerable.Empty<PodpisUzytkownikViewModel>();
        }

        public async Task<PodpisUzytkownikDto> Get(int pduID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            return await httpClient.GetFromJsonAsync<PodpisUzytkownikDto>($"PodpisUzytkownik/Get/{pduID}");
        }

        public async Task<ResponseMessage> Insert(PodpisUzytkownikDto pozycja)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(pozycja), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("PodpisUzytkownik/Insert", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var newObject = JsonConvert.DeserializeObject<PodpisUzytkownikDto>(apiResponse);

                    return new ResponseMessage((int)response.StatusCode, apiResponse, newObject?.pduID);
                }

                return new ResponseMessage((int)response.StatusCode, apiResponse, null);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        public async Task<ResponseMessage> Update(int pduID, PodpisUzytkownikDto pozycja)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(pozycja), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"PodpisUzytkownik/Update/{pduID}", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, pduID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, pduID);
            }
        }

        public async Task<ResponseMessage> Delete(int pduID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                var response = await httpClient.DeleteAsync($"PodpisUzytkownik/Delete/{pduID}");
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, pduID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }
    }
}
