
using GestorDeClubes.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestorDeClubes.MVC.Seed
{
    public static class UsuarioPruebaSeeder
    {
        public static async Task InicializarAsync(
            IServiceProvider serviceProvider)
        {
            // Este inicializador nunca debe crear usuarios
            // fuera del entorno de desarrollo.
            var environment = serviceProvider
                .GetRequiredService<IWebHostEnvironment>();

            if (!environment.IsDevelopment())
            {
                return;
            }

            var configuration = serviceProvider
                .GetRequiredService<IConfiguration>();

            var correo = configuration["UsuarioPrueba:Correo"];
            var nombre = configuration["UsuarioPrueba:Nombre"];
            var apellidos = configuration["UsuarioPrueba:Apellidos"];
            var password = configuration["UsuarioPrueba:Password"];

            // Si falta algún valor, no crear el usuario.
            if (string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellidos) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Falta configurar el usuario de prueba " +
                    "en los User Secrets del proyecto MVC.");
            }

            correo = correo.Trim();

            // El usuario de prueba debe utilizar uno de
            // los dominios admitidos por el login.
            var partes = correo.Split('@');

            if (partes.Length != 2 ||
                string.IsNullOrWhiteSpace(partes[0]) ||
                !(partes[1].Equals(
                      "ufide.ac.cr",
                      StringComparison.OrdinalIgnoreCase)
                  ||
                  partes[1].Equals(
                      "ufidelitas.ac.cr",
                      StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    "El correo del usuario de prueba " +
                    "debe utilizar un dominio institucional válido.");
            }

            var userManager = serviceProvider
                .GetRequiredService<UserManager<Usuario>>();

            // Evitar crear la misma cuenta en cada ejecución.
            var usuarioExistente =
                await userManager.FindByEmailAsync(correo);

            if (usuarioExistente != null)
            {
                return;
            }

            // Crear la cuenta mediante Identity.
            // La contraseña se almacena como un hash,
            // no como texto plano.
            var usuario = new Usuario
            {
                UserName = correo,
                Email = correo,
                Nombre = nombre.Trim(),
                Apellidos = apellidos.Trim(),
                Estado = true,
                FechaCreacion = DateTime.UtcNow,
                EmailConfirmed = false
            };

            var resultado = await userManager.CreateAsync(
                usuario,
                password);

            if (!resultado.Succeeded)
            {
                var errores = string.Join(
                    "; ",
                    resultado.Errors.Select(
                        error => error.Description));

                throw new InvalidOperationException(
                    "No se pudo crear el usuario de prueba: " +
                    errores);
            }

            // Asignar exclusivamente el rol Estudiante.
            // No otorgar permisos administrativos
            // a una cuenta ficticia de desarrollo.
            var resultadoRol =
                await userManager.AddToRoleAsync(
                    usuario,
                    "Estudiante");

            if (!resultadoRol.Succeeded)
            {
                var errores = string.Join(
                    "; ",
                    resultadoRol.Errors.Select(
                        error => error.Description));

                throw new InvalidOperationException(
                    "Se creó el usuario, pero no se pudo " +
                    "asignar el rol Estudiante: " + errores);
            }
        }
    }
}