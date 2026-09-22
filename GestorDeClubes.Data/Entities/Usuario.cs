
using Microsoft.AspNetCore.Identity;

namespace GestorDeClubes.Data.Entities
{
    public class Usuario : IdentityUser<int>
    {
        public string Nombre { get; set; } = string.Empty;

        public string Apellidos { get; set; } = string.Empty;

        public bool Estado { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? UltimoAcceso { get; set; }
    }
}