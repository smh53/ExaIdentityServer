using Client1.Models;
using Client1.Services;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Client1.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IApiResourceHttpClient _apiResourceHttpClient;

        public ProductController(IConfiguration configuration, IApiResourceHttpClient apiResourceHttpClient)
        {
            _configuration = configuration;
            _apiResourceHttpClient = apiResourceHttpClient;
        }
        public async Task<IActionResult> Index()
        {
         
            //var discovery = await httpClient.GetDiscoveryDocumentAsync("https://localhost:7278");
            // if (discovery.IsError)
            // {
            //     throw new Exception(discovery.Error);
            // }
            //ClientCredentialsTokenRequest clientCredentialsTokenRequest = new ClientCredentialsTokenRequest
            //{
            //    Address = discovery.TokenEndpoint,
            //    ClientId = _configuration["Client:ClientId"],
            //    ClientSecret = _configuration["Client:ClientSecret"],
            //};
            //var token =  await httpClient.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest);

            var response =await (await _apiResourceHttpClient.GetHttpClient()).GetAsync("https://localhost:7045/api/product/products");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.ReasonPhrase);
            }
            var content = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<Product>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });
            return View(products);
        }
    }
}
