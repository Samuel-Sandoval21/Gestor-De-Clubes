using System.ComponentModel.DataAnnotations;

namespace GestorDeClubes.MVC.ViewModels
{
    public class UsuarioEditarViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "El correo institucional es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
        [Display(Name = "Correo institucional")]
        public string CorreoInstitucional { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar un rol.")]
        [Display(Name = "Rol")]
        public string Rol { get; set; } = string.Empty;
    }
}