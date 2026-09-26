namespace GestorDeClubes.Business.Models
{
    public class ResultadoOperacion
    {
        public bool Exitoso { get; set; }

        public string Mensaje { get; set; } = string.Empty;

        public static ResultadoOperacion Ok(string mensaje)
        {
            return new ResultadoOperacion
            {
                Exitoso = true,
                Mensaje = mensaje
            };
        }

        public static ResultadoOperacion Error(string mensaje)
        {
            return new ResultadoOperacion
            {
                Exitoso = false,
                Mensaje = mensaje
            };
        }
    }
}