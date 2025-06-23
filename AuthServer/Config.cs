using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using System.Security.Claims;

namespace AuthServer
{
    public static class Config
    {
        //Identity Servera Apileri tanıtıyorum
        //Apilerin hangi izinleri olduğunu tanıtıyorum
        //Apilerin hangi clientlara izin vereceğini tanıtıyorum
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("resource_api1", "api1")
                {
                    Scopes = new List<string>
                    {
                        "api1.read",
                        "api1.write",
                        "api1.update",
                        "api1.delete",
                        "api1.all"

                    },
                    ApiSecrets = new [] {new Secret("secret1".Sha256())} // introspect 
                },
                new ApiResource("resource_api2", "api2")
                {
                    Scopes = new List<string>
                    {
                        "api2.read",
                        "api2.write",
                        "api2.update",
                        "api2.delete",
                        "api2.all"

                    },
                    ApiSecrets = new [] {new Secret("secret2".Sha256())} // introspect 
                }
            };
        }
        public static IEnumerable<ApiScope> GetApiScopes() {
            return new List<ApiScope>
            {
                new ApiScope("api1.read", "Read access to API 1"),
                new ApiScope("api1.write", "Write access to API 1"),
                new ApiScope("api1.update", "Update access to API 1"),
                new ApiScope("api1.delete", "Delete access to API 1"),
                new ApiScope("api1.all", "Full access to API 1"),

                new ApiScope("api2.read", "Read access to API 2"),
                new ApiScope("api2.write", "Write access to API 2"),
                new ApiScope("api2.update", "Update access to API 2"),
                new ApiScope("api2.delete", "Delete access to API 2"),
                new ApiScope("api2.all", "Full access to API 2")
            };

        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client()
                {
                    ClientId = "client1",
                    ClientName = "Client 1 API uygulaması",
                    AllowedGrantTypes = GrantTypes.ClientCredentials, // client id client secret zımbırtısı. Genelde bunu kullanıyoruz
                    ClientSecrets = { new Secret("password1".Sha256()) },
                    AllowedScopes = { "api1.read"}, //eğer örneğin resource_api2 için herhangi bir izni yoksa, tokenda aud içinde resource_api2 görünmez (otomatik oluyor bu)
                },

                new Client()
                {
                    ClientId = "client2",
                    ClientName = "Client 2 API uygulaması",
                    AllowedGrantTypes = GrantTypes.ClientCredentials, // client id client secret zımbırtısı. Genelde bunu kullanıyoruz
                    ClientSecrets = { new Secret("password2".Sha256()) },
                    AllowedScopes = { "api1.read","api2.write","api2.update",}, // ekstra scope bazlı yetkilendirme yapmazsan 2 apideki endpointleri de çalıştırabilir
                },


                new Client()
                {
                    ClientId = "Client1-Mvc",
                    RequirePkce = false, // PKCE kullanmıyoruz çünkü client credentials flow kullanıyoruz. PKCE sadece authorization code flow için gerekli
                    ClientName = "Client1 Mvc Uygulaması",
                    AllowedGrantTypes = GrantTypes.Hybrid, // code + credentials kullandigimiz icin hibrit
                    ClientSecrets = { new Secret("password1".Sha256()) },
                    RedirectUris = { "https://localhost:7002/signin-oidc" }, //clientlara kurulan openid paketi ile bu url olusuyor
                    AllowedScopes = {IdentityServerConstants.StandardScopes.OpenId,IdentityServerConstants.StandardScopes.OfflineAccess,IdentityServerConstants.StandardScopes.Profile, "api1.read", "CountryAndCity","Role", IdentityServerConstants.StandardScopes.Email}, // İzin verilen scope'lar. openid, id yi verir. profile, user ile ilgili bilgileri verir (family_name vs)
                    AllowOfflineAccess = true, // Refresh token 
                    AccessTokenLifetime = 3600, // refresh token lifetime
                    RefreshTokenUsage = TokenUsage.ReUse,
                    AbsoluteRefreshTokenLifetime = 2592000, // 30 days
                    RefreshTokenExpiration = TokenExpiration.Absolute, // gunu geldiginde omru dolar. sliding= gunu gelmeden kullanırsan omru uzar 
                    PostLogoutRedirectUris = { "https://localhost:7002/signout-callback-oidc" },
                    RequireConsent = true
                },

                  new Client()
                {
                    ClientId = "Client2-Mvc",
                    RequirePkce = false, // PKCE kullanmıyoruz çünkü client credentials flow kullanıyoruz. PKCE sadece authorization code flow için gerekli
                    ClientName = "Client2 Mvc Uygulaması",
                    AllowedGrantTypes = GrantTypes.Hybrid, // code + credentials kullandigimiz icin hibrit
                    ClientSecrets = { new Secret("password2".Sha256()) },
                    RedirectUris = { "https://localhost:7039/signin-oidc" }, //clientlara kurulan openid paketi ile bu url olusuyor
                    AllowedScopes = {IdentityServerConstants.StandardScopes.OpenId,IdentityServerConstants.StandardScopes.OfflineAccess,IdentityServerConstants.StandardScopes.Profile, "api2.read", "CountryAndCity","Role"}, // İzin verilen scope'lar. openid, id yi verir. profile, user ile ilgili bilgileri verir (family_name vs)
                    AllowOfflineAccess = true, // Refresh token 
                    AccessTokenLifetime = 3600, // refresh token lifetime
                    RefreshTokenUsage = TokenUsage.ReUse,
                    AbsoluteRefreshTokenLifetime = 2592000, // 30 days
                    RefreshTokenExpiration = TokenExpiration.Absolute, // gunu geldiginde omru dolar. sliding= gunu gelmeden kullanırsan omru uzar 
                    PostLogoutRedirectUris = { "https://localhost:7039/signout-callback-oidc" },
                    RequireConsent = false
                }

            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.Email(),
                new IdentityResources.OpenId(), // OpenID Connect protokolü için gerekli olan kimlik doğrulama bilgilerini sağlar (örn ID)
                new IdentityResources.Profile(), // Kullanıcı profili bilgilerini sağlar (örn ad, soyad, doğum tarihi)
                new IdentityResource()
                {
                    Name = "CountryAndCity",
                    DisplayName = "Country and City",
                    Description = "Country and City information of the user",
                    UserClaims = new [] {"country", "city" }
                },
                new IdentityResource()
                {
                    Name = "Role",
                    DisplayName = "Role",
                    Description = "Role information of the user",
                    UserClaims = [JwtClaimTypes.Role] // Rol bilgilerini iceren claim
                }
            };
        }

        public static IEnumerable<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser()
                {
                    SubjectId = "1", // Bu kullanıcı için benzersiz bir kimlik
                    Username = "testuser1",
                    Password = "password1",
                    Claims = new List<Claim>()
                    {
                        new Claim(JwtClaimTypes.GivenName, "TestSemih"),
                        new Claim(JwtClaimTypes.FamilyName, "Tavukcu"),
                        new Claim("country","Türkiye" ),
                        new Claim("city", "Ankara"),
                        new Claim(JwtClaimTypes.Role, "Admin"), 
                    }


                },
                 new TestUser()
                {
                    SubjectId = "2", // Bu kullanıcı için benzersiz bir kimlik
                    Username = "testuser2",
                    Password = "password2",
                    Claims = new List<Claim>()
                    {
                        new Claim(JwtClaimTypes.GivenName, "TestDumen"),
                        new Claim(JwtClaimTypes.FamilyName, "Dumen"),
                        new Claim("country","Türkiye" ),
                        new Claim("city", "İstanbul"),
                        new Claim(JwtClaimTypes.Role, "Customer"), 
                    }
                },

            };
        }
    }
}
