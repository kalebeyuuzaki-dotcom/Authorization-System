using Microsoft.EntityFrameworkCore;
using Portifolio.Models.Data;
using Portifolio.Models.Domain;
using Portifolio.Models.Result;

namespace Portifolio.Services.PasswordRecovery
{
    public class ConfirmCodePasswordRecoverService : IConfirmCodePasswordRecoverService
    {
        private const int MaxFailedAttempts = 3;
        private const string MaxAttemptsMessage = "Número máximo de tentativas excedido. Solicite um novo código.";
        private const string InvalidCodeMessage = "Código inválido. Tente novamente.";
        private const string ExpiredCodeMessage = "Este código de verificação expirou. Solicite um novo envio.";

        private readonly AppDbContext _context;

        public ConfirmCodePasswordRecoverService(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<PreRegisterResult> ValidateCodePasswordRecoverAsync(string email, string code)
        {
            var normalizedEmail = email.Trim().ToLower();

            var request = await _context.RecoverPasswordRequests
                .Where(p => p.Email == normalizedEmail)
                .OrderByDescending(p => p.ExpiresAt)
                .FirstOrDefaultAsync();

            if (request == null)
            {
                return PreRegisterResult.Failure(InvalidCodeMessage);
            }

            if (request.AccessFailedCount >= MaxFailedAttempts)
            {
                return PreRegisterResult.Failure(MaxAttemptsMessage);
            }

            if (request.ExpiresAt < DateTime.UtcNow)
            {
                return PreRegisterResult.Failure(ExpiredCodeMessage);
            }

            if (request.VerificationCode != code)
            {
                return await RegisterFailedAttemptAsync(request);
            }

            return PreRegisterResult.Success();
        }

        private async Task<PreRegisterResult> RegisterFailedAttemptAsync(RecoverPasswordRequest request)
        {
            request.AccessFailedCount += 1;
            await _context.SaveChangesAsync();

            var remainingAttempts = MaxFailedAttempts - request.AccessFailedCount;

            return remainingAttempts <= 0
                ? PreRegisterResult.Failure(MaxAttemptsMessage)
                : PreRegisterResult.Failure(InvalidCodeMessage);
        }
    }
}