using Microsoft.EntityFrameworkCore;
using Portifolio.Models.Data;
using Portifolio.Models.ViewModels;
namespace Portifolio.Services.Authentication
{
    public class LoginService : IAuthenticationService
    {
        private readonly AppDbContext _context;

        public LoginService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AuthenticateAsync(LoginViewModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => EF.Functions.Like(u.Email, model.Email));

            if (user == null)
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
        }
    }
}