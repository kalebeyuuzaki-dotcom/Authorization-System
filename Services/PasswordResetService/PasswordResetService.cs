using Microsoft.EntityFrameworkCore;
using Portifolio.Models.Data;
using Portifolio.Models.Domain;
using Portifolio.Models.Result;
using Portifolio.Models.ViewModels;

namespace Portifolio.Services.PasswordResetService
{
    public class PasswordResetService : IPasswordResetService
    {
        private const string FailOperationSQL = "Ocorreu um erro interno ao processar a recuperação da sua senha. Tente novamente.";
        private readonly AppDbContext _context;

        public PasswordResetService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PreRegisterResult> SaveNewPasswordAsync(ResetPasswordViewModel model)
        {
            string normalizedEmail = model.Email.ToLower();
            string hashPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            var request = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == normalizedEmail);

            if (request == null)
            {
                return PreRegisterResult.Failure(FailOperationSQL);
            }

            if (!await TrySaveNewPasswordAsync(request, hashPassword))
            {
                return PreRegisterResult.Failure(FailOperationSQL);
            }

            return PreRegisterResult.Success();

        }

        public async Task<bool> TrySaveNewPasswordAsync(User request, string newPassword)
        {
            try
            {
                request.Password = newPassword;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}