using Microsoft.EntityFrameworkCore;
using Portifolio.Models.Data;
using Portifolio.Models.Domain;
using Portifolio.Models.Result;
using Portifolio.Models.ViewModels;
using Portifolio.Services.Email;
using Portifolio.Utils;
using Portifolio.Utils.Templates;

namespace Portifolio.Services.PasswordRecovery
{
    public class RecoverPassword : IRecoverPassword
    {
        private const int CodeExpirationMinutes = 5;
        private const string SubjectEmail = "Seu Código de Recuperação de Senha";
        private const string FailSentEmail = "Não foi possível enviar o código de verificação agora. Por favor, tente novamente em alguns instantes.";
        private const string FailOperationSQL = "Ocorreu um erro interno ao processar a recuperação da sua senha. Tente novamente.";

        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public RecoverPassword(AppDbContext appDbContext, IEmailService emailService)
        {
            _context = appDbContext;
            _emailService = emailService;
        }

        public async Task<PreRegisterResult> GenerateCodeAndSendEmailAsync(EmailViewModel model)
        {
            string normalizedEmail = model.Email.ToLower();

            if (!await _context.Users.AnyAsync(u => u.Email == normalizedEmail))
            {
                return PreRegisterResult.Success();
            }

            var request = CreateRecoverPasswordRequest(normalizedEmail);

            var emailBody = EmailTemplates.BuildRecoverCodeEmail(
                request.VerificationCode,
                CodeExpirationMinutes
            );

            var emailSent = await _emailService.SendEmailAsync(
                request.Email,
                SubjectEmail,
                emailBody
            );

            if (!emailSent)
            {
                return PreRegisterResult.Failure(FailSentEmail);
            }

            if (!await TrySaveRecoverPasswordAsync(request))
            {
                return PreRegisterResult.Failure(FailOperationSQL);
            }

            return PreRegisterResult.Success();
        }

        public static RecoverPasswordRequest CreateRecoverPasswordRequest(string email)
        {
            return new RecoverPasswordRequest
            {
                Email = email,
                VerificationCode = GenerateCodeHelper.GenerateConfirmationCode(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(CodeExpirationMinutes),
                AccessFailedCount = 0,
            };
        }

        private async Task<bool> TrySaveRecoverPasswordAsync(RecoverPasswordRequest recoverPasswordRequest)
        {
            try
            {
                await _context.RecoverPasswordRequests.AddAsync(recoverPasswordRequest);
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