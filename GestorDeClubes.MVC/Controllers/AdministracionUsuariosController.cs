using GestorDeClubes.Data.Entities;
using GestorDeClubes.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorDeClubes.MVC.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdministracionUsuariosController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<Rol> _roleManager;

        public AdministracionUsuariosController(
            UserManager<Usuario> userManager,
            RoleManager<Rol> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ==========================================
        // HU-07: CONSULTAR USUARIOS
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> Index(
            string? busqueda)
        {
            var usuariosQuery = _userManager.Users
                .AsNoTracking()
                .AsQueryable();

            // Escenario 2: búsqueda específica.
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                busqueda = busqueda.Trim();

                usuariosQuery = usuariosQuery.Where(usuario =>
                    usuario.Nombre.Contains(busqueda) ||
                    usuario.Apellidos.Contains(busqueda) ||
                    usuario.Email!.Contains(busqueda));
            }

            var usuarios = await usuariosQuery
                .OrderBy(usuario => usuario.Nombre)
                .ThenBy(usuario => usuario.Apellidos)
                .ToListAsync();

            var modelo = new UsuarioListaViewModel
            {
                Busqueda = busqueda
            };

            foreach (var usuario in usuarios)
            {
                var roles =
                    await _userManager.GetRolesAsync(usuario);

                modelo.Usuarios.Add(
                    new UsuarioListaItemViewModel
                    {
                        Id = usuario.Id,

                        NombreCompleto =
                            $"{usuario.Nombre} {usuario.Apellidos}",

                        CorreoInstitucional =
                            usuario.Email ?? string.Empty,

                        Rol = roles.FirstOrDefault()
                            ?? "Sin rol",

                        Estado = usuario.Estado
                    });
            }

            return View(modelo);
        }

        // ==========================================
        // HU-06: MOSTRAR CREACIÓN
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            ViewBag.Roles = await ObtenerNombresRoles();

            return View(new UsuarioCrearViewModel());
        }

        // ==========================================
        // HU-06: CREAR USUARIO
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            UsuarioCrearViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await ObtenerNombresRoles();

                return View(model);
            }

            var correo = model.CorreoInstitucional.Trim();

            // Validar dominio institucional.
            if (!EsCorreoInstitucional(correo))
            {
                ModelState.AddModelError(
                    nameof(model.CorreoInstitucional),
                    "Debe utilizar un correo institucional válido.");

                ViewBag.Roles = await ObtenerNombresRoles();

                return View(model);
            }

            // Validar que el correo no exista.
            var usuarioExistente =
                await _userManager.FindByEmailAsync(correo);

            if (usuarioExistente != null)
            {
                ModelState.AddModelError(
                    nameof(model.CorreoInstitucional),
                    "Ya existe un usuario registrado con este correo.");

                ViewBag.Roles = await ObtenerNombresRoles();

                return View(model);
            }

            // Validar que el rol exista.
            if (!await _roleManager.RoleExistsAsync(model.Rol))
            {
                ModelState.AddModelError(
                    nameof(model.Rol),
                    "El rol seleccionado no existe.");

                ViewBag.Roles = await ObtenerNombresRoles();

                return View(model);
            }

            var usuario = new Usuario
            {
                UserName = correo,
                Email = correo,
                Nombre = model.Nombre.Trim(),
                Apellidos = model.Apellidos.Trim(),
                Estado = true,
                FechaCreacion = DateTime.UtcNow,
                EmailConfirmed = false
            };

            var resultado =
                await _userManager.CreateAsync(
                    usuario,
                    model.Password);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                ViewBag.Roles = await ObtenerNombresRoles();

                return View(model);
            }

            var resultadoRol =
                await _userManager.AddToRoleAsync(
                    usuario,
                    model.Rol);

            if (!resultadoRol.Succeeded)
            {
                // Si la creación funcionó pero el rol falló,
                // eliminamos el usuario para no dejar
                // un registro incompleto.
                await _userManager.DeleteAsync(usuario);

                foreach (var error in resultadoRol.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                ViewBag.Roles = await ObtenerNombresRoles();

                return View(model);
            }

            TempData["MensajeExito"] =
                "El usuario fue creado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // HU-08: MOSTRAR EDICIÓN
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuario =
                await _userManager.FindByIdAsync(id.ToString());

            if (usuario == null)
            {
                return NotFound();
            }

            var roles =
                await _userManager.GetRolesAsync(usuario);

            var modelo = new UsuarioEditarViewModel
            {
                Id = usuario.Id,
                CorreoInstitucional =
                    usuario.Email ?? string.Empty,
                Nombre = usuario.Nombre,
                Apellidos = usuario.Apellidos,
                Rol = roles.FirstOrDefault() ?? string.Empty
            };

            ViewBag.Roles = await ObtenerNombresRoles();

            return View(modelo);
        }

        // ==========================================
        // HU-08: ACTUALIZAR USUARIO
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            UsuarioEditarViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await ObtenerNombresRoles();

                return View(model);
            }

            var usuario =
                await _userManager.FindByIdAsync(
                    model.Id.ToString());

            if (usuario == null)
            {
                return NotFound();
            }

            var correo = model.CorreoInstitucional.Trim();

            if (!EsCorreoInstitucional(correo))
            {
                ModelState.AddModelError(
                    nameof(model.CorreoInstitucional),
                    "Debe utilizar un correo institucional válido.");

                ViewBag.Roles = await ObtenerNombresRoles();

                return View(model);
            }

            // Comprobar que el correo no pertenezca
            // a otro usuario.
            var usuarioConCorreo =
                await _userManager.FindByEmailAsync(correo);

            if (usuarioConCorreo != null &&
                usuarioConCorreo.Id != usuario.Id)
            {
                ModelState.AddModelError(
                    nameof(model.CorreoInstitucional),
                    "Ya existe otro usuario con este correo.");

                ViewBag.Roles = await ObtenerNombresRoles();

                return View(model);
            }

            if (!await _roleManager.RoleExistsAsync(model.Rol))
            {
                ModelState.AddModelError(
                    nameof(model.Rol),
                    "El rol seleccionado no existe.");

                ViewBag.Roles = await ObtenerNombresRoles();

                return View(model);
            }

            usuario.Email = correo;
            usuario.UserName = correo;
            usuario.Nombre = model.Nombre.Trim();
            usuario.Apellidos = model.Apellidos.Trim();

            var resultado =
                await _userManager.UpdateAsync(usuario);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                ViewBag.Roles = await ObtenerNombresRoles();

                return View(model);
            }

            // Actualizar el rol.
            var rolesActuales =
                await _userManager.GetRolesAsync(usuario);

            if (rolesActuales.Any())
            {
                var resultadoRemover =
                    await _userManager.RemoveFromRolesAsync(
                        usuario,
                        rolesActuales);

                if (!resultadoRemover.Succeeded)
                {
                    foreach (var error in resultadoRemover.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            error.Description);
                    }

                    ViewBag.Roles =
                        await ObtenerNombresRoles();

                    return View(model);
                }
            }

            var resultadoAgregar =
                await _userManager.AddToRoleAsync(
                    usuario,
                    model.Rol);

            if (!resultadoAgregar.Succeeded)
            {
                foreach (var error in resultadoAgregar.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                ViewBag.Roles =
                    await ObtenerNombresRoles();

                return View(model);
            }

            TempData["MensajeExito"] =
                "El usuario fue actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // HU-09: ACTIVAR O DESACTIVAR USUARIO
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var usuario = await _userManager.FindByIdAsync(id.ToString());

            if (usuario == null)
            {
                return NotFound();
            }

            // Evitar que el administrador desactive su propia cuenta.
            var administradorActual = await _userManager.GetUserAsync(User);

            if (administradorActual != null &&
                administradorActual.Id == usuario.Id &&
                usuario.Estado)
            {
                TempData["MensajeError"] =
                    "No podés desactivar tu propia cuenta de administrador.";

                return RedirectToAction(nameof(Index));
            }

            // Alternar entre activo e inactivo.
            bool nuevoEstado = !usuario.Estado;

            usuario.Estado = nuevoEstado;

            var resultado = await _userManager.UpdateAsync(usuario);

            if (!resultado.Succeeded)
            {
                TempData["MensajeError"] =
                    "No fue posible actualizar el estado del usuario.";

                return RedirectToAction(nameof(Index));
            }

            TempData["MensajeExito"] = nuevoEstado
                ? "El usuario fue activado correctamente."
                : "El usuario fue desactivado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // ROLES
        // ==========================================

        private async Task<List<string>> ObtenerNombresRoles()
        {
            return await _roleManager.Roles
                .OrderBy(rol => rol.Name)
                .Select(rol => rol.Name!)
                .ToListAsync();
        }

        // ==========================================
        // VALIDACIÓN DE CORREO INSTITUCIONAL
        // ==========================================

        private static bool EsCorreoInstitucional(
            string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                return false;
            }

            var partes = correo.Split('@');

            if (partes.Length != 2 ||
                string.IsNullOrWhiteSpace(partes[0]))
            {
                return false;
            }

            var dominio = partes[1];

            return dominio.Equals(
                       "ufide.ac.cr",
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   dominio.Equals(
                       "ufidelitas.ac.cr",
                       StringComparison.OrdinalIgnoreCase);
        }
    }
}

