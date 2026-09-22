
using System.ComponentModel.DataAnnotations;

namespace GestorDeClubes.MVC.ViewModels
{
    public class OlvidePasswordViewModel
    {
        [Required(ErrorMessage = "El correo institucional es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingresá un correo electrónico válido.")]
        [Display(Name = "Correo institucional")]
        public string CorreoInstitucional { get; set; } = string.Empty;
    }
}