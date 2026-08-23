using indigoERP.Domain.Dtos._Podpis;
using indigoERP.Domain.Enumerators._Podpis;
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
    public class PodpisService : IPodpisService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public PodpisService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<IEnumerable<PodpisViewModel>> Select(string pdpStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<PodpisViewModel>>($"Podpis/Select?pdpStatus={pdpStatus}");

            return result ?? Enumerable.Empty<PodpisViewModel>();
        }

        public async Task<IEnumerable<PodpisEnumerator>> SelectLookup(string pdpStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<PodpisEnumerator>>($"Podpis/SelectLookup?pdpStatus={pdpStatus}");

            return result ?? Enumerable.Empty<PodpisEnumerator>();
        }

        public async Task<PodpisDto> Get(int pdpID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            return await httpClient.GetFromJsonAsync<PodpisDto>($"Podpis/Get/{pdpID}");
        }

        public async Task<ResponseMessage> Update(int pdpID, PodpisDto podpis)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(podpis), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"Podpis/Update/{pdpID}", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, pdpID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, pdpID);
            }
        }

        public async Task<ResponseMessage> Insert(PodpisDto podpis)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(podpis), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("Podpis/Insert", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var newObject = JsonConvert.DeserializeObject<PodpisDto>(apiResponse);

                    return new ResponseMessage((int)response.StatusCode, apiResponse, newObject?.pdpID);
                }

                return new ResponseMessage(600, apiResponse, null);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        public async Task<ResponseMessage> Delete(int pdpID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                var response = await httpClient.DeleteAsync($"Podpis/Delete/{pdpID}");
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, pdpID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        public async Task<string> Preview(int usrID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<string>($"Podpis/Preview/{usrID}");

            return result ?? string.Empty;
        }
    }
}
