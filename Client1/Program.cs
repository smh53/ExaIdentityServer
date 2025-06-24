using Client1.Services;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor(); // HttpContext'i inject edebilmek icin
builder.Services.AddHttpClient(); // HttpContext'i inject edebilmek icin
builder.Services.AddScoped<IApiResourceHttpClient,ApiResourceHttpClient>(); 
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultScheme = "Cookies"; // 2 farkli uyelik sistemi( müsteri veya personel vs) varsa diye semalar kullanilir
   // opts.DefaultChallengeScheme = "oidc"; identity serverdan login ettiriyosak 
}).AddCookie("Cookies", opts =>
{
    opts.LoginPath = "/Login/Index"; // resourceowner flowunda yaptigimiz logine yonlendirme
    opts.AccessDeniedPath = "/Home/AccessDenied"; 
})


.AddOpenIdConnect("oidc", opts =>
{
    opts.SignInScheme = "Cookies";
    opts.Authority = "https://localhost:7278"; // IdentityServer Authority URL
    opts.ClientId = "Client1-Mvc";
    opts.ClientSecret = "password1";
    opts.ResponseType = "code id_token"; // Authorization Code Flow
    opts.GetClaimsFromUserInfoEndpoint = true;
    opts.SaveTokens = true; // Access token ve refresh token'i saklamak icin
    opts.Scope.Add("api1.read"); // izin verilen scope'lar
    opts.Scope.Add("offline_access"); // Refresh token
    opts.Scope.Add("Role");
    opts.Scope.Add("email");

    opts.Scope.Add("CountryAndCity"); //custom scope

    opts.ClaimActions.MapUniqueJsonKey("country", "country"); // Custom claim mapping
    opts.ClaimActions.MapUniqueJsonKey("city", "city"); 
    opts.ClaimActions.MapUniqueJsonKey("role", "role"); 

    opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
       NameClaimType = "name", // controllerda falan User.Identity.Name ile alabilmek icin mapledik. Ama User.Claims.FirstOrDefault(x => x.Type == "name") ile de alinabilir orda
        RoleClaimType = "role" // authorize attribute ile rol yetkilendirmesinde kullanmak icin maplenen role claimactionunu verdik
    };
});

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
