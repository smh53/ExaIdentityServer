using Duende.IdentityModel;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthServer.Services
{
    public class CustomProfileService(ICustomUserRepository _customUserRepository) : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            //userinfo calistiginda calisacak metod

            var subjectId = context.Subject.GetSubjectId();
            var user = _customUserRepository.FindById(int.Parse(subjectId)).Result;
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("name", user.UserName),
                new Claim("city", user.City),

            };

            if (user.Id == 1) // rol tablosu olabilirdi 
            {
                claims.Add(new Claim(JwtClaimTypes.Role, "Admin"));
            }
            else if (user.Id == 2)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, "User"));
            }
            else if (user.Id == 3)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, "Customer"));
            }
            context.AddRequestedClaims(claims);
            //context.IssuedClaims = claims; claimleri tokenda gelsin diyosan ac. Ama en iyisi userinfo endpointinden almak, tokeni sisirmemek
            return Task.CompletedTask;
        }
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var userId = context.Subject.GetSubjectId();
            var user = await _customUserRepository.FindById(int.Parse(userId));

           context.IsActive = user != null && !string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(user.Password);
        }
    }
    
}
