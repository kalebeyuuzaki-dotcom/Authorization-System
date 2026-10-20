using Microsoft.EntityFrameworkCore;
using Portifolio.Models.Data;
using Portifolio.Models.Domain;
using Portifolio.Models.Result;
using Portifolio.Models.ViewModels;
using Portifolio.Services.Email;
using Portifolio.Utils;
using Portifolio.Utils.Templates;

namespace Portifolio.Services.Register
{
    public class RegisterService : IRegisterService
    {
        private const int CodeExpirationMinutes = 5;
        private const string FailOperationSQL = "Ocorreu um erro interno ao processar seu cadastro. Tente novamente.";
        private const string FailSentEmail = "Não foi possível enviar o código de verificação agora. Por favor, tente novamente em alguns instantes.";
        private const string SubjectEmail = "Seu Código de Confirmação";

        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public RegisterService(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<PreRegisterResult> GenerateCodeAndSentEmailAsync(RegisterViewModel model)
        {
            var normalizedEmail = model.Email.ToLower();

            if (await _context.Users.AnyAsync(u => u.Email == normalizedEmail))
            {
                return PreRegisterResult.Success();
            }

            var request = CreatePreRegisterRequest(model, normalizedEmail);
        
            var emailBody = EmailTemplates.BuildVerificationCodeEmail(
                request.Name, request.VerificationCode, CodeExpirationMinutes
            );

            var emailSent = await _emailService.SendEmailAsync(
                normalizedEmail,
                SubjectEmail,
                emailBody
            );

            if (!emailSent)
            {
                return PreRegisterResult.Failure(FailSentEmail);
            }

            if (!await TrySavePreRegisterAsync(request))
            {
                return PreRegisterResult.Failure(FailOperationSQL);
            }

            return PreRegisterResult.Success();
        }

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
    }
}