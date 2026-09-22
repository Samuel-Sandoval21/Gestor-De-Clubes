
using GestorDeClubes.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestorDeClubes.MVC.Seed
{
    public static class AdministradorPruebaSeeder
    {
        public static async Task InicializarAsync(
            IServiceProvider serviceProvider)
        {
            // Crear la cuenta únicamente en Development.
            var environment = serviceProvider
                .GetRequiredService<IWebHostEnvironment>();

            if (!environment.IsDevelopment())
            {
                return;
            }

            var configuration = serviceProvider
                .GetRequiredService<IConfiguration>();

            var correo = configuration["AdministradorPrueba:Correo"];
            var nombre = configuration["AdministradorPrueba:Nombre"];
            var apellidos = configuration["AdministradorPrueba:Apellidos"];
            var password = configuration["AdministradorPrueba:Password"];

            // Si no se configuró la cuenta administrativa,
            // no crearla.
            if (string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellidos) ||
                string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            correo = correo.Trim();

            // Validar el dominio institucional.
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
                    "El correo del administrador de prueba " +
                    "debe utilizar un dominio institucional válido.");
            }

            var userManager = serviceProvider
                .GetRequiredService<UserManager<Usuario>>();

            // No modificar una cuenta que ya exista.
            var usuarioExistente =
                await userManager.FindByEmailAsync(correo);

            if (usuarioExistente != null)
            {
                // No otorgar permisos administrativos
                // automáticamente a usuarios existentes.
                return;
            }

            // Crear una cuenta administrativa independiente.
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
                    "No se pudo crear el administrador " +
                    "de prueba: " + errores);
            }

            // Asignar el rol Administrador.
            var resultadoRol =
                await userManager.AddToRoleAsync(
                    usuario,
                    "Administrador");

            if (!resultadoRol.Succeeded)
            {
                var errores = string.Join(
                    "; ",
                    resultadoRol.Errors.Select(
                        error => error.Description));

                throw new InvalidOperationException(
                    "Se creó el administrador de prueba, " +
                    "pero no se pudo asignar su rol: " + errores);
            }
        }
    }
}