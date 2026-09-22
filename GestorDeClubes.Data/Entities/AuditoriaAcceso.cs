
using System;

namespace GestorDeClubes.Data.Entities
{
    public class AuditoriaAcceso
    {
        // Identificador único del registro de auditoría.
        public int AuditoriaAccesoID { get; set; }

        // Usuario que realizó la acción.
        public int UsuarioID { get; set; }

        // Tipo de evento: INGRESO o CIERRE.
        public string TipoEvento { get; set; } = string.Empty;

        // Fecha y hora del evento, almacenadas en UTC.
        public DateTime FechaHora { get; set; }

        // Relación con la tabla USUARIOS.
        public Usuario Usuario { get; set; } = null!;
    }
}