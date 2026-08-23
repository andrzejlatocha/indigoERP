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
    public class PodpisGrupaService : IPodpisGrupaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public PodpisGrupaService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<IEnumerable<PodpisGrupaViewModel>> SelectByPodpis(int pdpID, string pdgStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<PodpisGrupaViewModel>>($"PodpisGrupa/SelectByPodpis/{pdpID}?pdgStatus={pdgStatus}");

            return result ?? Enumerable.Empty<PodpisGrupaViewModel>();
        }

        public async Task<PodpisGrupaDto> Get(int pdgID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            return await httpClient.GetFromJsonAsync<PodpisGrupaDto>($"PodpisGrupa/Get/{pdgID}");
        }

        public async Task<ResponseMessage> Insert(PodpisGrupaDto pozycja)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(pozycja), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("PodpisGrupa/Insert", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var newObject = JsonConvert.DeserializeObject<PodpisGrupaDto>(apiResponse);

                    return new ResponseMessage((int)response.StatusCode, apiResponse, newObject?.pdgID);
                }

                return new ResponseMessage((int)response.StatusCode, apiResponse, null);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        public async Task<ResponseMessage> Update(int pdgID, PodpisGrupaDto pozycja)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(pozycja), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"PodpisGrupa/Update/{pdgID}", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, pdgID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, pdgID);
            }
        }

        public async Task<ResponseMessage> Delete(int pdgID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                var response = await httpClient.DeleteAsync($"PodpisGrupa/Delete/{pdgID}");
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, pdgID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }
    }
}
