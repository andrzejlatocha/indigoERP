using System.Text;
using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.Enumerators._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;
using indigoERP.Service.Extensions;
using indigoERP.Service.Interfaces._Wspolne;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace indigoERP.Service.Services._Wspolne
{
    public class WiadomoscPracownikService : IWiadomoscPracownikService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public WiadomoscPracownikService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<IEnumerable<WiadomoscZakresEnumerator>> SelectZakresy(int appID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<WiadomoscZakresEnumerator>>($"WiadomoscZakres/Select?appID={appID}");

            return result ?? Enumerable.Empty<WiadomoscZakresEnumerator>();
        }

        public async Task<IEnumerable<WiadomoscZakresPracownikViewModel>> SelectPracownicyZakres(int wzaID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<WiadomoscZakresPracownikViewModel>>(
                $"WiadomoscPracownik/SelectByWiadomoscZakres/{wzaID}");

            return result ?? Enumerable.Empty<WiadomoscZakresPracownikViewModel>();
        }

        public async Task<IEnumerable<WiadomoscWzorzecEnumerator>> SelectWzorce(int appID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<WiadomoscWzorzecEnumerator>>(
                $"WiadomoscWzorzec/SelectLookup?msgAplikacjaID={appID}&msgStatus=2");

            return result ?? Enumerable.Empty<WiadomoscWzorzecEnumerator>();
        }

        public async Task<(ResponseMessage Response, WiadomoscWysylkaZakresWynikModel? Wynik)> InsertWysylka(
            WiadomoscWysylkaZakresModel model, IReadOnlyList<WiadomoscPlikWysylki> pliki)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                using var content = new MultipartFormDataContent
                {
                    { new StringContent(model.wiaAplikacjaID.ToString()), nameof(model.wiaAplikacjaID) },
                    { new StringContent(model.wiaTemat), nameof(model.wiaTemat) },
                    { new StringContent(model.wiaTresc), nameof(model.wiaTresc) },
                    { new StringContent(model.wzaID.ToString()), nameof(model.wzaID) }
                };

                foreach (var prcID in model.prcIDs)
                {
                    content.Add(new StringContent(prcID.ToString()), nameof(model.prcIDs));
                }

                foreach (var plik in pliki)
                {
                    var zawartosc = new ByteArrayContent(plik.Dane);
                    zawartosc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                    content.Add(zawartosc, "pliki", plik.Nazwa);
                }

                var response = await httpClient.PostAsync("WiadomoscPracownik/InsertWysylka", content);
                var apiResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return (new ResponseMessage((int)response.StatusCode, apiResponse, null), null);
                }

                var wynik = JsonConvert.DeserializeObject<WiadomoscWysylkaZakresWynikModel>(apiResponse);

                return (new ResponseMessage((int)response.StatusCode, string.Empty, wynik?.wiaID), wynik);
            }
            catch (Exception ex)
            {
                return (new ResponseMessage(600, ex.Message, null), null);
            }
        }
        public async Task<(ResponseMessage Response, WiadomoscDto? Wiadomosc)> InsertRobocza(
            WiadomoscRoboczaModel model, IReadOnlyList<WiadomoscPlikWysylki> pliki)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                using var content = new MultipartFormDataContent
                {
                    { new StringContent(model.wiaAplikacjaID.ToString()), nameof(model.wiaAplikacjaID) },
                    { new StringContent(model.wiaTemat), nameof(model.wiaTemat) },
                    { new StringContent(model.wiaTresc), nameof(model.wiaTresc) }
                };

                if (model.wiaWzorzecID is { } wzorzecID)
                {
                    content.Add(new StringContent(wzorzecID.ToString()), nameof(model.wiaWzorzecID));
                }

                DodajPliki(content, pliki);

                var response = await httpClient.PostAsync("WiadomoscPracownik/InsertRobocza", content);
                var apiResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return (new ResponseMessage((int)response.StatusCode, apiResponse, null), null);
                }

                var wiadomosc = JsonConvert.DeserializeObject<WiadomoscDto>(apiResponse);

                return (new ResponseMessage((int)response.StatusCode, string.Empty, wiadomosc?.wiaID), wiadomosc);
            }
            catch (Exception ex)
            {
                return (new ResponseMessage(600, ex.Message, null), null);
            }
        }

        public async Task<IEnumerable<WiadomoscDto>> SelectRobocze(int appID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<WiadomoscDto>>($"WiadomoscPracownik/SelectRobocze?appID={appID}");

            return result ?? Enumerable.Empty<WiadomoscDto>();
        }

        public async Task<(ResponseMessage Response, WiadomoscWysylkaZakresWynikModel? Wynik)> WyslijRobocza(
            int wiaID, WiadomoscWyslijRoboczaModel model)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                using var content = new MultipartFormDataContent
                {
                    { new StringContent(model.wzaID.ToString()), nameof(model.wzaID) }
                };

                foreach (var prcID in model.prcIDs)
                {
                    content.Add(new StringContent(prcID.ToString()), nameof(model.prcIDs));
                }

                if (!string.IsNullOrWhiteSpace(model.kodHarmonogram))
                {
                    content.Add(new StringContent(model.kodHarmonogram), nameof(model.kodHarmonogram));

                    if (model.dataHarmonogramStart is DateTime start)
                    {
                        content.Add(new StringContent(start.ToString("o")), nameof(model.dataHarmonogramStart));
                    }

                    if (model.dataHarmonogramKoniec is DateTime koniec)
                    {
                        content.Add(new StringContent(koniec.ToString("o")), nameof(model.dataHarmonogramKoniec));
                    }
                }

                var response = await httpClient.PostAsync($"WiadomoscPracownik/WyslijRobocza/{wiaID}", content);
                var apiResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return (new ResponseMessage((int)response.StatusCode, apiResponse, null), null);
                }

                var wynik = JsonConvert.DeserializeObject<WiadomoscWysylkaZakresWynikModel>(apiResponse);

                return (new ResponseMessage((int)response.StatusCode, string.Empty, wynik?.wiaID), wynik);
            }
            catch (Exception ex)
            {
                return (new ResponseMessage(600, ex.Message, null), null);
            }
        }

        public async Task<IEnumerable<WiadomoscHarmonogramViewModel>> SelectHarmonogram(int appID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<WiadomoscHarmonogramViewModel>>(
                $"WiadomoscHarmonogram/SelectAktywne?appID={appID}");

            return result ?? Enumerable.Empty<WiadomoscHarmonogramViewModel>();
        }

        public async Task<ResponseMessage> AnulujHarmonogram(int wiaID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                var response = await httpClient.PostAsync($"WiadomoscHarmonogram/Anuluj/{wiaID}", null);
                var tresc = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, response.IsSuccessStatusCode ? string.Empty : tresc, null);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        public async Task<ResponseMessage> PoprawRobocza(int wiaID, WiadomoscPoprawaModel model)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                var response = await httpClient.PutAsJsonAsync($"WiadomoscPracownik/Popraw/{wiaID}", model);
                var tresc = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, response.IsSuccessStatusCode ? string.Empty : tresc, null);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        public async Task<ResponseMessage> UsunWiadomosc(int wiaID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                var response = await httpClient.DeleteAsync($"WiadomoscPracownik/Usun/{wiaID}");
                var tresc = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, response.IsSuccessStatusCode ? string.Empty : tresc, null);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        private static void DodajPliki(MultipartFormDataContent content, IReadOnlyList<WiadomoscPlikWysylki> pliki)
        {
            foreach (var plik in pliki)
            {
                var zawartosc = new ByteArrayContent(plik.Dane);
                zawartosc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                content.Add(zawartosc, "pliki", plik.Nazwa);
            }
        }

        public async Task<IEnumerable<WiadomoscPodsumowanieDto>> SelectPodsumowanie(int appID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<WiadomoscPodsumowanieDto>>($"WiadomoscPracownik/SelectPodsumowanie?appID={appID}");

            return result ?? Enumerable.Empty<WiadomoscPodsumowanieDto>();
        }
        public async Task<IEnumerable<WiadomoscPracownikViewModel>> SelectWyslane(int appID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<WiadomoscPracownikViewModel>>($"WiadomoscPracownik/SelectWyslane?appID={appID}");

            return result ?? Enumerable.Empty<WiadomoscPracownikViewModel>();
        }
    }
}