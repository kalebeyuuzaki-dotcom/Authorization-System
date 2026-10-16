using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Portifolio.Models.Data;
using Portifolio.Models.Domain;
using Portifolio.Models.Result;
using Portifolio.Models.ViewModels;
using Portifolio.Services.Email;
using Portifolio.Utils;

namespace Portifolio.Services.Register
{
    public class RegisterService : IRegisterService
    {
        private const int CodeExpirationMinutes = 5;
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

            /* SUCESSO FALSO: redireciona o usuário para a view de confirmação
            sem gerar código no banco (evitando revelar que o e-mail já existe). */
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

        // ===============================================================
        // Helpers: pré-cadastro
        // ===============================================================

        private static PreRegisterRequest CreatePreRegisterRequest(RegisterViewModel model, string email)
        {
            return new PreRegisterRequest
            {
                Id = Guid.NewGuid(),
                Name = model.Name.ToUpper(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                VerificationCode = GenerateCodeHelper.GenerateConfirmationCode(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(CodeExpirationMinutes),
                AccessFailedCount = 0,
                IsVerified = false
            };
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
    }
}