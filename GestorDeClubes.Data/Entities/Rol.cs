
using Microsoft.AspNetCore.Identity;

namespace GestorDeClubes.Data.Entities
{
    public class Rol : IdentityRole<int>
    {
        public string Descripcion { get; set; } = string.Empty;

        public bool Estado { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaModificacion { get; set; }
    }
}