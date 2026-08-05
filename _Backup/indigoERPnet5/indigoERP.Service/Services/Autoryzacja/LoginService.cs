using indigoERP.Domain.Models._Autoryzacja;
using indigoERP.Domain.ViewModels._Autoryzacja;
using indigoERP.Service.Interfaces._Autoryzacja;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace indigoERP.Service.Services._Autoryzacja
{
    public class LoginService : ILoginService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public LoginService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<UzytkownikViewModel> Get(UzytkownikLoginModel uzytkownikLoginModel)
        {
            if (_httpClient.BaseAddress == null)
            {
                string? uri = _configuration["appSettings:IndigoErpUrl"];
                if (uri != null)
                {
                    _httpClient.BaseAddress = new Uri(uri);
                }
            }

            try
            {
                var uzytkownik = await _httpClient.GetFromJsonAsync<UzytkownikViewModel>($"Login/Get?usrNazwa={uzytkownikLoginModel.usrNazwa}&usrHaslo={uzytkownikLoginModel.usrHaslo}");

                return uzytkownik;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UzytkownikViewModel> Get(string usrNazwa, string usrHaslo)
        {
            if (_httpClient.BaseAddress == null)
            {
                string? uri = _configuration["appSettings:IndigoErpUrl"];
                if (uri != null)
                {
                    _httpClient.BaseAddress = new Uri(uri);
                }
            }

            try
            {
                var uzytkownik = await _httpClient.GetFromJsonAsync<UzytkownikViewModel>($"Login/Get?usrNazwa={usrNazwa}&usrHaslo={usrHaslo}");

                return uzytkownik;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
