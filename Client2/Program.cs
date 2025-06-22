using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddAuthentication(opts =>
{
    opts.DefaultScheme = "Cookies"; // 2 farkli uyelik sistemi( müsteri veya personel vs) varsa diye semalar kullanilir
    opts.DefaultChallengeScheme = "oidc";
}).AddCookie("Cookies", opts =>
{
    opts.AccessDeniedPath = "/Home/AccessDenied";
})
.AddOpenIdConnect("oidc", opts =>
{
    opts.SignInScheme = "Cookies";
    opts.Authority = "https://localhost:7278"; // IdentityServer Authority URL
    opts.ClientId = "Client2-Mvc";
    opts.ClientSecret = "password2";
    opts.ResponseType = "code id_token"; // Authorization Code Flow
    opts.GetClaimsFromUserInfoEndpoint = true;
    opts.SaveTokens = true; // Access token ve refresh token'i saklamak icin
    opts.Scope.Add("api1.read"); // izin verilen scope'lar
    opts.Scope.Add("offline_access"); // Refresh token
    opts.Scope.Add("Role");

    opts.Scope.Add("CountryAndCity"); //custom scope

    opts.ClaimActions.MapUniqueJsonKey("country", "country"); // Custom claim mapping
    opts.ClaimActions.MapUniqueJsonKey("city", "city");
    opts.ClaimActions.MapUniqueJsonKey("role", "role");

    opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {

        RoleClaimType = "role" // authorize attribute ile rol yetkilendirmesinde kullanmak icin maplenen role claimactionunu verdik
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
