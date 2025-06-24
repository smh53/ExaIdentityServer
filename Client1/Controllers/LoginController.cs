using Client1.Models;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Client1.Controllers
{
    public class LoginController : Controller 
    {
        // GET: LoginController
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel loginViewModel)
        {
            var client = new HttpClient();
            var discoveryDocument = await client.GetDiscoveryDocumentAsync("https://localhost:7278");

            if(discoveryDocument.IsError)
            {
                ModelState.AddModelError("", discoveryDocument.Error);
                return View(loginViewModel);
            }
            var password = new PasswordTokenRequest() 
            {
                Address = discoveryDocument.TokenEndpoint,
                Password = loginViewModel.Password,
                UserName = loginViewModel.Email,
                ClientId = "Client1-ResourceOwner-Mvc",
                ClientSecret = "password-ro",


            };

          var token =  await client.RequestPasswordTokenAsync(password);
            if (token.IsError)
            {
                ModelState.AddModelError("", "Email veya kullanıcı adı yanlış");
                return View(loginViewModel);
            }
       var userInfo =   await client.GetUserInfoAsync(new UserInfoRequest
          {
              Address = discoveryDocument.UserInfoEndpoint,
              Token = token.AccessToken
          });
            ClaimsIdentity identity = new ClaimsIdentity(userInfo.Claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", "role");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            var authhenticationProperties = new AuthenticationProperties();

            authhenticationProperties.StoreTokens(new List<AuthenticationToken>() 
            {
                  new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.ExpiresIn,
                    Value = DateTime.UtcNow.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture) // ISO 8601 format
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = token.AccessToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = token.RefreshToken
                }
            });

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authhenticationProperties);



            return RedirectToAction("Index", "User");


        }

       
    }
}
