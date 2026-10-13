using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Portifolio.Models.Data;
using Portifolio.Models.Domain;
using Portifolio.Models.Result;
using Portifolio.Models.ViewModels;
using Portifolio.Services.Email;

namespace Portifolio.Services.Register
{
    public class RegisterService : IRegisterService
    {
        private const int MaxFailedAttempts = 3;
        private const int CodeExpirationMinutes = 5;
        private const int CodeMinValue = 100000;
        private const int CodeMaxValueExclusive = 1000000;
        private const string MaxAttemptsMessage = "Número máximo de tentativas excedido. Solicite um novo código.";

        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public RegisterService(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<PreRegisterResult> PreRegisterAsync(RegisterViewModel model)
        {
            var email = model.Email.ToLower();

            // Sucesso falso: redireciona o usuário para a view de confirmação
            // sem gerar código no banco (evita revelar que o e-mail já existe).
            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                return PreRegisterResult.Success();
            }

            var preRegister = CreatePreRegisterRequest(model, email);

            if (!await TrySavePreRegisterAsync(preRegister))
            {
                return PreRegisterResult.Failure("Ocorreu um erro interno ao processar seu cadastro. Tente novamente.");
            }

            var emailSent = await _emailService.SendEmailAsync(
                email,
                "Seu Código de Confirmação",
                BuildConfirmationEmail(model.Name, preRegister.VerificationCode));

            if (!emailSent)
            {
                await TryRemovePreRegisterAsync(preRegister);
                return PreRegisterResult.Failure("Não foi possível enviar o código de verificação agora. Por favor, tente novamente em alguns instantes.");
            }

            return PreRegisterResult.Success();
        }

        public async Task<PreRegisterResult> VerifyCodeAsync(string email, string code)
        {
            var normalizedEmail = email.ToLower();

            var preRegister = await _context.PreRegisterRequests
                .Where(p => p.Email == normalizedEmail && !p.IsVerified)
                .OrderByDescending(p => p.ExpiresAt)
                .FirstOrDefaultAsync();

            if (preRegister == null)
            {
                return PreRegisterResult.Failure("Código inválido.");
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
        // Helpers: pré-cadastro
        // ---------------------------------------------------------------

        private static PreRegisterRequest CreatePreRegisterRequest(RegisterViewModel model, string email)
        {
            return new PreRegisterRequest
            {
                Id = Guid.NewGuid(),
                Name = model.Name.ToUpper(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                VerificationCode = GenerateConfirmationCode(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(CodeExpirationMinutes),
                AccessFailedCount = 0,
                IsVerified = false
            };
        }

        private static string GenerateConfirmationCode()
        {
            return RandomNumberGenerator.GetInt32(CodeMinValue, CodeMaxValueExclusive).ToString();
        }

        private static string BuildConfirmationEmail(string name, string code)
        {
            return $@"
            <h3>Olá, {name}!</h3>
            <p>Obrigado por se cadastrar. Use o código abaixo para confirmar seu e-mail:</p>
            <h2 style='color: #2b7de9; letter-spacing: 2px;'>{code}</h2>
            <p>Este código expira em {CodeExpirationMinutes} minutos.</p>";
        }

        private async Task<bool> TrySavePreRegisterAsync(PreRegisterRequest preRegister)
        {
            try
            {
                await _context.PreRegisterRequests.AddAsync(preRegister);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task TryRemovePreRegisterAsync(PreRegisterRequest preRegister)
        {
            try
            {
                _context.PreRegisterRequests.Remove(preRegister);
                await _context.SaveChangesAsync();
            }
            catch
            {
                // Falha silenciosa: o registro apenas vai expirar naturalmente por tempo no banco.
            }
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
                : PreRegisterResult.Failure($"Código incorreto. Você ainda tem {remainingAttempts} tentativa(s).");
        }

        // Salva o usuário definitivo no banco e remove o pré-cadastro em uma única transação.
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
                _context.PreRegisterRequests.Remove(preRegister);

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