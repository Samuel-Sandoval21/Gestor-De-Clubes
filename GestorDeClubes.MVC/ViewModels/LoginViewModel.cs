
using System.ComponentModel.DataAnnotations;

namespace GestorDeClubes.MVC.ViewModels
{
    public class LoginViewModel
    {
        // ==========================================
        // CORREO INSTITUCIONAL
        // ==========================================

        [Required(
            ErrorMessage = "El correo institucional es obligatorio.")]
        [EmailAddress(
            ErrorMessage = "Ingrese un correo electrónico válido.")]
        [Display(Name = "Correo institucional")]
        public string CorreoInstitucional { get; set; } = string.Empty;

        // ==========================================
        // CONTRASEÑA
        // ==========================================

        [Required(
            ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        // ==========================================
        // RECORDAR SESIÓN
        // ==========================================

        [Display(Name = "Recordar sesión")]
        public bool Recordarme { get; set; } = false;
    }
}