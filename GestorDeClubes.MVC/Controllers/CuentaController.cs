
using GestorDeClubes.Data.Entities;
using GestorDeClubes.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GestorDeClubes.MVC.Controllers
{
    public class CuentaController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IWebHostEnvironment _environment;

        public CuentaController(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
        }

        // ==========================================
        // HU-01: MOSTRAR FORMULARIO DE LOGIN
        // ==========================================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            // Si ya existe una sesión, ir al inicio.
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        // ==========================================
        // HU-01: PROCESAR INICIO DE SESIÓN
        // ==========================================

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var correo = model.CorreoInstitucional.Trim();

            if (!EsCorreoInstitucional(correo))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Debe utilizar un correo institucional válido.");

                return View(model);
            }

            var usuario =
                await _userManager.FindByEmailAsync(correo);

            if (usuario == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Las credenciales ingresadas son inválidas.");

                return View(model);
            }

            if (!usuario.Estado)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "La cuenta se encuentra inactiva.");

                return View(model);
            }

            var resultado =
                await _signInManager.PasswordSignInAsync(
                    usuario,
                    model.Password,
                    model.Recordarme,
                    lockoutOnFailure: true);

            if (resultado.Succeeded)
            {
                usuario.UltimoAcceso = DateTime.UtcNow;

                await _userManager.UpdateAsync(usuario);

                return RedirectToAction("Index", "Home");
            }

            if (resultado.IsLockedOut)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "La cuenta está bloqueada temporalmente. " +
                    "Intente nuevamente más tarde.");

                return View(model);
            }

            if (resultado.IsNotAllowed)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No se permite el acceso a esta cuenta.");

                return View(model);
            }

            ModelState.AddModelError(
                string.Empty,
                "Las credenciales ingresadas son inválidas.");

            return View(model);
        }

        // ==========================================
        // HU-02: CERRAR SESIÓN
        // ==========================================

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login", "Cuenta");
        }

        // ==========================================
        // HU-03: MOSTRAR RECUPERACIÓN DE CONTRASEÑA
        // ==========================================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult OlvidePassword()
        {
            return View(new OlvidePasswordViewModel());
        }

        // ==========================================
        // HU-03: PROCESAR SOLICITUD DE RECUPERACIÓN
        // ==========================================

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OlvidePassword(
            OlvidePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var correo = model.CorreoInstitucional.Trim();

            if (!EsCorreoInstitucional(correo))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Debe utilizar un correo institucional válido.");

                return View(model);
            }

            var usuario =
                await _userManager.FindByEmailAsync(correo);

            // Mantener una respuesta pública genérica.
            if (usuario != null && usuario.Estado)
            {
                var token =
                    await _userManager.GeneratePasswordResetTokenAsync(
                        usuario);

                // Simulación exclusiva del entorno de desarrollo.
                if (_environment.IsDevelopment())
                {
                    var enlaceRecuperacion = Url.Action(
                        action: "RestablecerPassword",
                        controller: "Cuenta",
                        values: new
                        {
                            correo = usuario.Email,
                            token
                        },
                        protocol: Request.Scheme);

                    TempData["EnlaceRecuperacionDesarrollo"] =
                        enlaceRecuperacion;
                }
            }

            return RedirectToAction(
                "RecuperacionSolicitada",
                "Cuenta");
        }

        // ==========================================
        // HU-03: CONFIRMACIÓN DE SOLICITUD
        // ==========================================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RecuperacionSolicitada()
        {
            return View();
        }

        // ==========================================
        // HU-03: MOSTRAR FORMULARIO DE NUEVA CONTRASEÑA
        // ==========================================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RestablecerPassword(
            string? correo,
            string? token)
        {
            // El enlace debe contener ambos datos.
            if (string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(
                    "El enlace de recuperación es inválido.");
            }

            var model = new RestablecerPasswordViewModel
            {
                Correo = correo,
                Token = token
            };

            return View(model);
        }

        // ==========================================
        // HU-03: GUARDAR NUEVA CONTRASEÑA
        // ==========================================

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestablecerPassword(
            RestablecerPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var correo = model.Correo.Trim();

            // Validar que la solicitud corresponda a un
            // correo institucional.
            if (!EsCorreoInstitucional(correo))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "El enlace de recuperación es inválido.");

                return View(model);
            }

            var usuario =
                await _userManager.FindByEmailAsync(correo);

            // No revelar si el correo corresponde a una cuenta.
            if (usuario == null || !usuario.Estado)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "El enlace de recuperación es inválido " +
                    "o ya no está disponible.");

                return View(model);
            }

            // Identity valida el token y aplica la política
            // de seguridad a la nueva contraseña.
            var resultado =
                await _userManager.ResetPasswordAsync(
                    usuario,
                    model.Token,
                    model.NuevaPassword);

            if (resultado.Succeeded)
            {
                // Si el usuario tenía una sesión abierta,
                // finalizarla antes de volver al login.
                await _signInManager.SignOutAsync();

                return RedirectToAction(
                    "PasswordRestablecida",
                    "Cuenta");
            }

            // Mostrar los errores devueltos por Identity.
            foreach (var error in resultado.Errors)
            {
                if (error.Code == "InvalidToken")
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "El enlace de recuperación es inválido, " +
                        "ha expirado o ya fue utilizado.");
                }
                else
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }
            }

            return View(model);
        }

        // ==========================================
        // HU-03: CONFIRMACIÓN DE CAMBIO EXITOSO
        // ==========================================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult PasswordRestablecida()
        {
            // Crearemos esta vista en el siguiente paso.
            return View();
        }

        // ==========================================
        // VALIDACIÓN DE CORREOS INSTITUCIONALES
        // ==========================================

        private static bool EsCorreoInstitucional(string correo)
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