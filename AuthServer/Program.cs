using AuthServer;
using AuthServer.Models;
using AuthServer.Repository;
using AuthServer.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<CustomDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentityServer()
    .AddInMemoryApiResources(Config.GetApiResources())
    .AddInMemoryApiScopes(Config.GetApiScopes())
    .AddInMemoryClients(Config.GetClients())
    .AddInMemoryIdentityResources(Config.GetIdentityResources())
  //  .AddTestUsers(Config.GetUsers().ToList())
  .AddProfileService<CustomProfileService>() // Custom profile service for user claims
    .AddDeveloperSigningCredential(); // For development purposes only, use a real certificate in production

builder.Services.AddRazorPages(); // quickstart ui uses razor pages, without this, pages will not be found

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

builder.Services.AddScoped<ICustomUserRepository, CustomUserRepository>();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();


}
app.MapRazorPages(); // quickstart ui uses razor pages, without this, pages will not be found
app.UseHttpsRedirection();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
