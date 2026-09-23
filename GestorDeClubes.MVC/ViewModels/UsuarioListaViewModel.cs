namespace GestorDeClubes.MVC.ViewModels
{
    public class UsuarioListaViewModel
    {
        public string? Busqueda { get; set; }

        public List<UsuarioListaItemViewModel> Usuarios { get; set; }
            = new();
    }

    public class UsuarioListaItemViewModel
    {
        public int Id { get; set; }

        public string NombreCompleto { get; set; }
            = string.Empty;

        public string CorreoInstitucional { get; set; }
            = string.Empty;

        public string Rol { get; set; }
            = string.Empty;

        public bool Estado { get; set; }

        public string EstadoTexto =>
            Estado ? "Activo" : "Inactivo";
    }
}