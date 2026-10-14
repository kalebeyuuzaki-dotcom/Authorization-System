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
    <div style='background-color: #f8f9fa; padding: 40px 10px; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; text-align: center;'>
        <table align='center' border='0' cellpadding='0' cellspacing='0' style='max-width: 500px; width: 100%; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 6px rgba(0,0,0,0.05); border: 1px solid #e9ecef; overflow: hidden;'>
            <tr>
                <td style='padding: 40px 30px; text-align: left;'>
                    
                    <!-- Saudação -->
                    <h3 style='color: #212529; font-size: 20px; font-weight: 700; margin-top: 0; margin-bottom: 16px;'>
                        Olá, {name}!
                    </h3>
                    
                    <!-- Texto de Instrução -->
                    <p style='color: #495057; font-size: 15px; line-height: 1.6; margin-top: 0; margin-bottom: 24px;'>
                        Obrigado por se cadastrar em nosso sistema. Use o código de verificação abaixo para confirmar e ativar a sua conta:
                    </p>
                    
                    <!-- Bloco Centralizado do Código -->
                    <div style='text-align: center; margin-bottom: 24px; padding: 16px; background-color: #f8f9fa; border-radius: 6px; border: 1px dashed #ced4da;'>
                        <h2 style='color: #0d6efd; font-size: 32px; font-weight: 700; letter-spacing: 4px; margin: 0;'>
                            {code}
                        </h2>
                    </div>
                    
                    <!-- Aviso de Expiração -->
                    <p style='color: #6c757d; font-size: 13px; line-height: 1.5; margin-top: 0; margin-bottom: 0;'>
                        ⏱️ Este código expira em <strong>{CodeExpirationMinutes} minutos</strong>. Se você não solicitou este cadastro, pode ignorar esta mensagem com segurança.
                    </p>
                    
                </td>
            </tr>
            <!-- Rodapé sutil do e-mail -->
            <tr>
                <td style='background-color: #f1f3f5; padding: 16px 30px; text-align: center;'>
                    <p style='color: #adb5bd; font-size: 11px; margin: 0;'>
                        Esta é uma mensagem automática. Por favor, não responda a este e-mail.
                    </p>
                </td>
            </tr>
        </table>
    </div>";
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