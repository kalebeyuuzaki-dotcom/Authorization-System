using Microsoft.EntityFrameworkCore;
using Portifolio.Models.Data;
using Portifolio.Models.Domain;
using Portifolio.Models.Result;

namespace Portifolio.Services.Register
{
    public class ConfirmationCodeService : IConfirmationCodeService
    {
        private const int MaxFailedAttempts = 3;
        private const string MaxAttemptsMessage = "Número máximo de tentativas excedido. Solicite um novo código.";
        private readonly AppDbContext _context;

        public ConfirmationCodeService(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<PreRegisterResult> VerifyCodeAsync(string email, string code)
        {
            var normalizedEmail = email.ToLower();

            /* Se existir uma conta ativa (IsVerified = true), retorna mensagem de erro genérica ao usuário. */
            var preRegister = await _context.PreRegisterRequests
                .Where(p => p.Email == normalizedEmail && !p.IsVerified)
                .OrderByDescending(p => p.ExpiresAt)
                .FirstOrDefaultAsync();

            if (preRegister == null)
            {
                return PreRegisterResult.Failure("Código inválido. Tente novamente.");
            }

            if (preRegister.AccessFailedCount >= MaxFailedAttempts)
            {
                return PreRegisterResult.Failure(MaxAttemptsMessage);
            }

            if (preRegister.ExpiresAt < DateTime.UtcNow)
            {
                return PreRegisterResult.Failure("Este código de verificação expirou. Solicite um novo envio.");
            }

            if (preRegister.VerificationCode != code)
            {
                return await RegisterFailedAttemptAsync(preRegister);
            }

            return await CreateUserAsync(preRegister);
        }

        // ---------------------------------------------------------------
        // Helpers: verificação do código
        // ---------------------------------------------------------------

        private async Task<PreRegisterResult> RegisterFailedAttemptAsync(PreRegisterRequest preRegister)
        {
            preRegister.AccessFailedCount += 1;
            await _context.SaveChangesAsync();

            var remainingAttempts = MaxFailedAttempts - preRegister.AccessFailedCount;

            return remainingAttempts <= 0
                ? PreRegisterResult.Failure(MaxAttemptsMessage)
                : PreRegisterResult.Failure($"Código inválido. Tente novamente.");
        }

        private async Task<PreRegisterResult> CreateUserAsync(PreRegisterRequest preRegister)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = new User
                {
                    Name = preRegister.Name,
                    Email = preRegister.Email,
                    Password = preRegister.PasswordHash
                };

                await _context.Users.AddAsync(user);

                preRegister.IsVerified = true;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return PreRegisterResult.Success();
            }
            catch
            {
                await transaction.RollbackAsync();
                return PreRegisterResult.Failure("Erro ao finalizar a criação da sua conta. Tente novamente.");
            }
        }
    }
}