
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Client1.Services
{
    public class ApiResourceHttpClient : IApiResourceHttpClient
    {
        private readonly HttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _client;

        public ApiResourceHttpClient(HttpContextAccessor httpContextAccessor, HttpClient client)
        {
            _httpContextAccessor = httpContextAccessor;
            _client = client;
        }
        public async Task<HttpClient> GetHttpClient()
        {
           var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            if (!string.IsNullOrEmpty(accessToken))
            {
                _client.SetBearerToken(accessToken);
            }
            return _client;

        }
    }
}
