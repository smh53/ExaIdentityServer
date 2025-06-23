using AuthServer.Models;

namespace AuthServer.Services
{
    public interface ICustomUserRepository
    {
        Task<bool> ValidateUser(string email, string password);
        Task<CustomUser> FindById(int id);
        Task<CustomUser> FindByEmail(string userName);
        
    }
}
