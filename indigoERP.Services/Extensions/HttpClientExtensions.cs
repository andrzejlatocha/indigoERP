using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace indigoERP.Service.Extensions
{
    public static class HttpClientExtensions
    {
        public static HttpClient CreateClientWithToken(this HttpClient httpClient, string name, IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            httpClient = httpClientFactory.CreateClient("baseApiClient");

            if (authenticationStateProvider != null)
            {
                var authstate = Task.Run(async () => await authenticationStateProvider.GetAuthenticationStateAsync()).Result;
                if (authstate.User != null)
                {
                    string accessToken = authstate.User.FindFirst("accessToken").Value;
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                }
            }

            return httpClient;
        }

        public static HttpClient AddTokenToHeader(this HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider)
        {
            if (authenticationStateProvider != null)
            {
                var authstate = Task.Run(async () => await authenticationStateProvider.GetAuthenticationStateAsync()).Result;

                if (authstate.User != null)
                {
                    string accessToken = authstate.User.FindFirst("accessToken").Value;
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                }
            }

            return httpClient;
        }
    }
}
