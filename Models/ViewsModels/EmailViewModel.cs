using System.ComponentModel.DataAnnotations;

namespace Portifolio.Models.ViewModels
{
    public class EmailViewModel
    {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }
}