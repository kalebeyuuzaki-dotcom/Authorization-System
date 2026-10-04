using LoginSystemMvc.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Portifolio.Models.Data;
using Portifolio.Models.ViewModels;
using Portifolio.Services.Interfaces;
using BCrypt.Net;

namespace Portifolio.Services.Implements
{
    public class LoginService : IAuthService
    {
        private readonly AppDbContext _context;

        public LoginService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Método que verifica e valida credenciais E-mail e Senha do usuário no login.
        /// </summary>
        /// <param name="model">A Model das credenciais que serão verificadas e validadas.</param>
        /// <returns>Retorna true se E-email e Senha forem válidos e existentes no banco de dados.</returns>
        public async Task<bool> AuthAsync(LoginViewModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());

            if (user == null)
            {
                return false;
            }

            bool isPasswordValid = await VerifyPasswordAsync(model.Password, user.Password);

            if (isPasswordValid == false)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> VerifyPasswordAsync(string password, string hashedPassword)
        {
            return await Task.Run(() => BCrypt.Net.BCrypt.Verify(password.Trim(), hashedPassword.Trim()));
        }
    }
}