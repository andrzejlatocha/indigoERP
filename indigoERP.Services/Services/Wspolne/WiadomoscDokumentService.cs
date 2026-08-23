using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Service.Extensions;
using indigoERP.Service.Interfaces._Wspolne;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;

namespace indigoERP.Service.Services._Wspolne
{
    public class WiadomoscDokumentService : IWiadomoscDokumentService
    {
        private const long MaxFileSize = 25 * 1024 * 1024;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public WiadomoscDokumentService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<IEnumerable<WiadomoscDokumentDto>> SelectByWiadomosc(int wiaID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<WiadomoscDokumentDto>>($"WiadomoscDokument/SelectByWiadomosc/{wiaID}");

            return result ?? Enumerable.Empty<WiadomoscDokumentDto>();
        }

        public async Task<ResponseMessage> Upload(int wiaID, IBrowserFile file)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                using var content = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream(MaxFileSize);
                using var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType);

                content.Add(streamContent, "file", file.Name);
                content.Add(new StringContent(wiaID.ToString()), "wdoWiadomoscID");

                var response = await httpClient.PostAsync("WiadomoscDokument/Upload", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, wiaID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, wiaID);
            }
        }

        public async Task<WiadomoscDokumentFile?> Download(int wdoID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var response = await httpClient.GetAsync($"WiadomoscDokument/Open/{wdoID}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
                ?? response.Content.Headers.ContentDisposition?.FileName?.Trim('"')
                ?? $"dokument-{wdoID}";
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var stream = await response.Content.ReadAsStreamAsync();

            return new WiadomoscDokumentFile(stream, fileName, contentType);
        }

        public async Task<ResponseMessage> Delete(int wdoID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                var response = await httpClient.DeleteAsync($"WiadomoscDokument/Delete/{wdoID}");
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, wdoID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }
    }
}
