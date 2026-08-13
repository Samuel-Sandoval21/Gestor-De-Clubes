using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GestorDeClubes.MVC.Models;

namespace GestorDeClubes.MVC.Controllers;

using Microsoft.AspNetCore.Mvc;



    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // SIMULACIÓN: El rol se obtendría de la base de datos (Identity o tabla Usuario)
            // Cambia esto manualmente para probar diferentes vistas:
            // "Estudiante", "Coordinador", "Bienestar", "Admin"
            ViewBag.UserRole = "Estudiante";

            // Datos simulados para el Dashboard (Estadísticas)
            ViewBag.ClubesActivos = 12;
            ViewBag.EventosProximos = 4;
            ViewBag.MiembrosTotales = 185;

            return View();
        }
    }
