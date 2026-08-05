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
    public class SerwerService : ISerwerService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public SerwerService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<IEnumerable<SerwerViewModel>> Select(string srvStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<SerwerViewModel>>($"Serwer/Select?srvStatus={srvStatus}");

            return result ?? Enumerable.Empty<SerwerViewModel>();
        }

        public async Task<SerwerDto> Get(int srvID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            return await httpClient.GetFromJsonAsync<SerwerDto>($"Serwer/Get/{srvID}");
        }

        public async Task<ResponseMessage> Insert(SerwerDto serwer)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(serwer), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("Serwer/Insert", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var newObject = JsonConvert.DeserializeObject<SerwerDto>(apiResponse);

                    return new ResponseMessage((int)response.StatusCode, apiResponse, newObject?.srvID);
                }

                return new ResponseMessage(600, apiResponse, null);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        public async Task<ResponseMessage> Update(int srvID, SerwerDto serwer)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(serwer), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"Serwer/Update/{srvID}", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, srvID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, srvID);
            }
        }

        public async Task<ResponseMessage> Delete(int srvID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                var response = await httpClient.DeleteAsync($"Serwer/Delete/{srvID}");
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, srvID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        public async Task<SerwerInfoViewModel?> GetInfo()
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                return await httpClient.GetFromJsonAsync<SerwerInfoViewModel>("Serwer/GetInfo");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<MonitorSerwerStatusViewModel>> SelectStatus()
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<MonitorSerwerStatusViewModel>>("Serwer/SelectStatus");

            return result ?? Enumerable.Empty<MonitorSerwerStatusViewModel>();
        }

        public async Task<MonitorSerwerStatusViewModel?> Check(int srvID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                return await httpClient.GetFromJsonAsync<MonitorSerwerStatusViewModel>($"Serwer/Check/{srvID}");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<SqlAgentJobViewModel>> SelectJoby(int srvID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var response = await httpClient.GetAsync($"Serwer/SelectJoby/{srvID}");

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();

                throw new InvalidOperationException(ExtractProblemDetail(body) ?? $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}");
            }

            var result = await response.Content.ReadFromJsonAsync<IEnumerable<SqlAgentJobViewModel>>();

            return result ?? Enumerable.Empty<SqlAgentJobViewModel>();
        }

        // Kontroler zwraca błędy przez Problem(detail, statusCode) — treść odpowiedzi to ProblemDetails
        // JSON z polem "detail" zawierającym rzeczywisty powód (np. dlaczego SelectJoby dostało 502).
        private static string? ExtractProblemDetail(string responseBody)
        {
            try
            {
                var problem = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);

                return problem != null && problem.TryGetValue("detail", out var detail) ? detail?.ToString() : null;
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}
