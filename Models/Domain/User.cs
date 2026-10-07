using System.ComponentModel.DataAnnotations;

namespace Portifolio.Models.Domain
{
    public class User
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "O nome deve ter entre {2} e {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail informado é inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter no mínimo {2} caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", 
            ErrorMessage = "A senha deve conter pelo menos uma letra maiúscula, uma letra minúscula e um número.")]
        public string Password { get; set; } = string.Empty;
    }
}
