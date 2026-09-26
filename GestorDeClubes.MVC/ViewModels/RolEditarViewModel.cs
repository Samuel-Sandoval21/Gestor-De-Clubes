using System.ComponentModel.DataAnnotations;

namespace GestorDeClubes.MVC.ViewModels
{
    public class RolEditarViewModel
    {
        public int RolId { get; set; }

        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        [StringLength(
            50,
            ErrorMessage = "El nombre del rol no puede superar los 50 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción del rol es obligatoria.")]
        [StringLength(
            50,
            ErrorMessage = "La descripción no puede superar los 50 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        public bool EsRolBase { get; set; }
    }
}