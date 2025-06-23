using AuthServer.Models;
using AuthServer.Services;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Repository
{
    public class CustomUserRepository(CustomDbContext _context) : ICustomUserRepository
    {
        public async Task<CustomUser> FindByEmail(string userName)
        {
            return await _context.CustomUsers.FirstOrDefaultAsync(u => u.Email == userName);
           
        }

        public async Task<CustomUser> FindById(int id)
        {
           return await _context.CustomUsers.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> ValidateUser(string email, string password)
        {
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return false;
            }
            return await _context.CustomUsers.AnyAsync(u => u.Email == email && u.Password == password);
        }
    }
}
