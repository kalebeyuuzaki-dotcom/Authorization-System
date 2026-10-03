using System.ComponentModel.DataAnnotations;

namespace Portifolio.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
            ErrorMessage = "A senha deve conter pelo menos uma letra maiúscula, uma letra minúscula e um número.")]
        public string Password { get; set; } = string.Empty;
    }
}
