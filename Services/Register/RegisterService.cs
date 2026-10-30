using Hangfire;
using Microsoft.EntityFrameworkCore;
using Portifolio.Models.Data;
using Portifolio.Models.Domain;
using Portifolio.Models.Result;
using Portifolio.Models.ViewModels;
using Portifolio.Services.Jobs;
using Portifolio.Utils;
using Portifolio.Utils.Templates;

namespace Portifolio.Services.Register
{
    public class RegisterService : IRegisterService
    {
        private const int CodeExpirationMinutes = 5;
        private const string FailOperationSQL = "Ocorreu um erro interno ao processar seu cadastro. Tente novamente.";
        private const string SubjectEmail = "Seu Código de Confirmação";

        private readonly AppDbContext _context;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public RegisterService(AppDbContext context, IBackgroundJobClient backgroundJobClient)
        {
            _context = context;
            _backgroundJobClient = backgroundJobClient;
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
                request.Name,
                request.VerificationCode,
                CodeExpirationMinutes
            );

            try
            {
                _backgroundJobClient.Enqueue<IRegisterAccountJob>(job =>
                    job.TrySaveRequestAndSendEmailAsync(
                        normalizedEmail,
                        SubjectEmail,
                        emailBody,
                        request
                    )
                );
            }
            catch
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
    }
}