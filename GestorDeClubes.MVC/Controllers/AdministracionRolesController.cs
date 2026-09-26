using GestorDeClubes.Business.Interfaces;
using GestorDeClubes.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestorDeClubes.MVC.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdministracionRolesController : Controller
    {
        private readonly IRolService _rolService;

        public AdministracionRolesController(
            IRolService rolService)
        {
            _rolService = rolService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var roles = await _rolService.ObtenerTodosAsync();

            var modelo = roles
                .Select(r => new RolListaViewModel
                {
                    RolId = r.Id,
                    Nombre = r.Name ?? string.Empty,
                    Descripcion = r.Descripcion,
                    Estado = r.Estado,
                    EsRolBase = _rolService.EsRolBase(r.Name)
                })
                .ToList();

            return View(modelo);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View(new RolCrearViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            RolCrearViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var resultado = await _rolService.CrearAsync(
                modelo.Nombre,
                modelo.Descripcion);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                return View(modelo);
            }

            TempData["MensajeExito"] = resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }
    }
}