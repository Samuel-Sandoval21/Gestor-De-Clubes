using Microsoft.AspNetCore.Mvc;

namespace GestorDeClubes.MVC.Controllers
{
    public class PrototipoController : Controller
    {
        // Vista 1: Lista de Clubes Activos
        public IActionResult Clubes()
        {
            // Simulamos una lista de clubes para el prototipo
            ViewBag.Clubes = new List<string> { "Club de Fotografía", "Club de Ajedrez", "Club de Robótica", "Club de Debate", "Club de Volleyball" };
            return View();
        }

        // Vista 2: Login
        public IActionResult Login()
        {
            return View();
        }

        // Vista 3: Próximos Eventos (El calendario que ya diseñaste)
        // Nota: Ponemos el nombre "Eventos" para que el menú sea claro
        public IActionResult Eventos()
        {
            return View();
        }
    }
}