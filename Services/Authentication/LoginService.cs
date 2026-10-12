using Microsoft.EntityFrameworkCore;
using Portifolio.Models.Data;
using Portifolio.Models.ViewModels;

namespace Portifolio.Services.Authentication
{
    public class LoginService : ILoginService
    {
        private readonly AppDbContext _context;

        public LoginService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserSessionViewModel?> AuthenticateAsync(LoginViewModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return null;
            }

            return new UserSessionViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };
        }
    }
}
