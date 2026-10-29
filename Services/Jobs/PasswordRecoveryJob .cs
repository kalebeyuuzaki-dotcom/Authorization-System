using Portifolio.Models.Data;
using Portifolio.Models.Domain;
using Portifolio.Services.Email;

namespace Portifolio.Services.Jobs
{
    public class PasswordRecoveryJob : IPasswordRecoveryJob
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private const int CodeExpirationMinutes = 5;

        public PasswordRecoveryJob(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task SendEmailAndSaveRequestAsync(string email, string subject, string body, string code)
        {
            var request = new RecoverPasswordRequest
            {
                Email = email,
                VerificationCode = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(CodeExpirationMinutes),
                AccessFailedCount = 0
            };

            try
            {
                await _context.RecoverPasswordRequests.AddAsync(request);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao salvar no banco. O Hangfire reexecutará este job.", ex);
            }

            bool success = await _emailService.SendEmailAsync(email, subject, body);
            if (!success)
            {
                throw new Exception("Banco salvou, mas SMTP falhou. O Hangfire tentará reenviar.");
            }
        }

    }
}