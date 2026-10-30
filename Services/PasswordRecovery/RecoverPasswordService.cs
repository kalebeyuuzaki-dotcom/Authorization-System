using Hangfire;
using Microsoft.EntityFrameworkCore;
using Portifolio.Models.Data;
using Portifolio.Models.Result;
using Portifolio.Models.ViewModels;
using Portifolio.Services.Jobs;
using Portifolio.Utils;
using Portifolio.Utils.Templates;

namespace Portifolio.Services.PasswordRecovery
{
    public class RecoverPassword : IRecoverPassword
    {
        private const int CodeExpirationMinutes = 5;
        private const string SubjectEmail = "Seu Código de Recuperação de Senha";
        private const string FailOperationSQL = "Ocorreu um erro interno ao processar a recuperação da sua senha. Tente novamente.";

        private readonly AppDbContext _context;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public RecoverPassword(AppDbContext appDbContext, IBackgroundJobClient backgroundJobClient)
        {
            _context = appDbContext;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<PreRegisterResult> GenerateCodeAndSendEmailAsync(EmailViewModel model)
        {
            string normalizedEmail = model.Email.ToLower();

            if (!await _context.Users.AnyAsync(u => u.Email == normalizedEmail))
            {
                return PreRegisterResult.Success();
            }

            var verificationCode = GenerateCodeHelper.GenerateConfirmationCode();

            var emailBody = EmailTemplates.BuildRecoverCodeEmail(
                verificationCode,
                CodeExpirationMinutes
            );

            try
            {
                _backgroundJobClient.Enqueue<IPasswordRecoveryJob>(job =>
                    job.SendEmailAndSaveRequestAsync(
                        normalizedEmail, 
                        SubjectEmail, 
                        emailBody, 
                        verificationCode
                    )
                );
            }
            catch
            {
                return PreRegisterResult.Failure(FailOperationSQL);
            }

            return PreRegisterResult.Success();
        }
    }
}