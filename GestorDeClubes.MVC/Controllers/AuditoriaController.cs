
using GestorDeClubes.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorDeClubes.MVC.Controllers
{
    // HU-05: Solo los administradores pueden consultar
    // los registros de auditoría.
    [Authorize(Roles = "Administrador")]
    public class AuditoriaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditoriaController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // HU-05: CONSULTAR BITÁCORA DE ACCESOS
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Consultar los registros junto con sus usuarios.
            // AsNoTracking evita realizar seguimiento de cambios
            // porque esta pantalla es únicamente de lectura.

            var registros = await _context.AuditoriaAccesos
                .AsNoTracking()
                .Include(a => a.Usuario)
                .OrderByDescending(a => a.FechaHora)
                .ThenByDescending(a => a.AuditoriaAccesoID)
                .ToListAsync();

            return View(registros);
        }
    }
}