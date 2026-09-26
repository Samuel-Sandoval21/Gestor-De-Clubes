namespace GestorDeClubes.MVC.ViewModels
{
    public class RolListaViewModel
    {
        public int RolId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public bool Estado { get; set; }

        public bool EsRolBase { get; set; }
    }
}