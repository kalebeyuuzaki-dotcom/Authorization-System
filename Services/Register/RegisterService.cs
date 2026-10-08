using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Portifolio.Migrations;
using Portifolio.Models.Data;
using Portifolio.Models.Domain;
using Portifolio.Models.ViewModels;

namespace Portifolio.Services.Register
{
    public class RegisterService : IRegisterService
    {
        private readonly AppDbContext _context;
        public RegisterService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> PreRegisterAsync(RegisterViewModel model)
        {
            var verifyEmail = await _context.Users.AnyAsync(u => u.Email == model.Email);

            if (verifyEmail)
            {
                return false;
            }

            string confirmationCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var preRegister = new PreRegisterRequest
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Email = model.Email,
                PasswordHash = hashedPassword,
                VerificationCode = confirmationCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                AccessFailedCount = 0,
                IsVerified = false
            };

            await _context.PreRegisterRequests.AddAsync(preRegister);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}