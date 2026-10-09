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
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public RegisterService(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<PreRegisterResult> PreRegisterAsync(RegisterViewModel model)
        {
            var verifyEmail = await _context.Users.AnyAsync(u => u.Email == model.Email.ToLower());

            if (verifyEmail)
            {
                /* SUCESSO FALSO: REDIRECIONA O USUÁRIO PARA A VIEW DE CONFIRMAÇÃO 
                SEM GERAR CÓDIGO NO BANCO. */
                return PreRegisterResult.Success(); 
            }

            string confirmationCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var preRegister = new PreRegisterRequest
            {
                Id = Guid.NewGuid(),
                Name = model.Name.ToUpper(),
                Email = model.Email.ToLower(),
                PasswordHash = hashedPassword,
                VerificationCode = confirmationCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                AccessFailedCount = 0,
                IsVerified = false
            };

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.PreRegisterRequests.AddAsync(preRegister);
                await _context.SaveChangesAsync();

                string subject = "Seu Código de Confirmação";
                string htmlMessage = $@"
                <h3>Olá, {model.Name}!</h3>
                <p>Obrigado por se cadastrar. Use o código abaixo para confirmar seu e-mail:</p>
                <h2 style='color: #2b7de9; letter-spacing: 2px;'>{confirmationCode}</h2>
                <p>Este código expira em 5 minutos.</p>";

                bool emailSent = await _emailService.SendEmailAsync(model.Email, subject, htmlMessage);

                if (!emailSent)
                {
                    await transaction.RollbackAsync();
                    return PreRegisterResult.Failure("Não foi possível enviar o código de verificação agora. Por favor, tente novamente em alguns instantes.");
                }

                await transaction.CommitAsync();
                return PreRegisterResult.Success();
            }
            catch
            {
                await transaction.RollbackAsync();
                return PreRegisterResult.Failure("Ocorreu um erro interno ao processar seu cadastro. Tente novamente.");
            }
        }
    }
}
