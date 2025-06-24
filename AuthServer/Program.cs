using AuthServer;
using AuthServer.Models;
using AuthServer.Repository;
using AuthServer.Seeds;
using AuthServer.Services;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
var assemblyName = typeof(Program).Assembly.GetName().Name;


builder.Services.AddDbContext<CustomDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentityServer()
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            sql => sql.MigrationsAssembly(assemblyName));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            sql => sql.MigrationsAssembly(assemblyName));
    })
    //.AddInMemoryApiResources(Config.GetApiResources())
    //.AddInMemoryApiScopes(Config.GetApiScopes())
    //.AddInMemoryClients(Config.GetClients())
    //.AddInMemoryIdentityResources(Config.GetIdentityResources())
  //  .AddTestUsers(Config.GetUsers().ToList())
  .AddProfileService<CustomProfileService>() // Custom profile service for user claims

  .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>() // Custom resource owner password validator
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

// Seed the database with initial data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
    dbContext.Database.Migrate();
    IdentityServerSeedData.Seed(dbContext); // Seed the IdentityServer data
}

app.Run();
