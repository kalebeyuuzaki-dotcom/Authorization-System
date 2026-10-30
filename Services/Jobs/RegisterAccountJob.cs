using Portifolio.Models.Data;
using Portifolio.Models.Domain;
using Portifolio.Services.Email;

namespace Portifolio.Services.Jobs
{
    public class RegisterAccountJob : IRegisterAccountJob
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public RegisterAccountJob(AppDbContext appDbContext, IEmailService emailService)
        {
            _context = appDbContext;
            _emailService = emailService;
        }

        public async Task TrySaveRequestAndSendEmailAsync(string email, string subject, string body, PreRegisterRequest model)
        {
            try
            {
                await _context.PreRegisterRequests.AddAsync(model);
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