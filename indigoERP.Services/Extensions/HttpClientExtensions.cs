using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace indigoERP.Service.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpClient> CreateClientWithTokenAsync(this IHttpClientFactory httpClientFactory,
            AuthenticationStateProvider authenticationStateProvider)
        {
            var httpClient = httpClientFactory.CreateClient("baseApiClient");
            var authstate = await authenticationStateProvider.GetAuthenticationStateAsync();
            var accessToken = authstate.User?.FindFirst("accessToken")?.Value;

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            return httpClient;
        }

        public static async Task<HttpClient> AddTokenToHeaderAsync(this HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider)
        {
            var authstate = await authenticationStateProvider.GetAuthenticationStateAsync();
            var accessToken = authstate.User?.FindFirst("accessToken")?.Value;

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            return httpClient;
        }
    }
}
