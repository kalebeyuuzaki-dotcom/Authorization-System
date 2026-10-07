using System.ComponentModel.DataAnnotations;

namespace Portifolio.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ']+(\s+[a-zA-ZÀ-ÿ']+)+$",
            ErrorMessage = "O nome deve conter pelo menos duas palavras (nome e sobrenome).")]
        [Display(Name = "Nome")]
        public string Name { get; set; } = string.Empty;

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

        [Required(ErrorMessage = "Confirme sua senha.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "As senhas não coincidem.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
