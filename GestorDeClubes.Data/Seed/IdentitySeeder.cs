
using GestorDeClubes.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace GestorDeClubes.Data.Seed
{
    public static class IdentitySeeder
    {
        public static async Task InicializarRolesAsync(
            IServiceProvider serviceProvider)
        {
            // Obtener el administrador de roles de Identity.
            var roleManager = serviceProvider
                .GetRequiredService<RoleManager<Rol>>();

            // ==========================================
            // ROLES DEL GESTOR DE CLUBES
            // ==========================================

            // Orden definido por el equipo:
            // 1. Administrador
            // 2. Personal de Bienestar Estudiantil
            // 3. Asistente de Club
            // 4. Estudiante

            var roles = new[]
            {
                new
                {
                    Nombre = "Administrador",
                    Descripcion = "Administración general del sistema."
                },
                new
                {
                    Nombre = "Personal de Bienestar Estudiantil",
                    Descripcion = "Gestión de actividades de Bienestar Estudiantil."
                },
                new
                {
                    Nombre = "Asistente de Club",
                    Descripcion = "Líder de club encargado de apoyar la gestión de su club."
                },
                new
                {
                    Nombre = "Estudiante",
                    Descripcion = "Acceso a las funcionalidades estudiantiles."
                }
            };

            // ==========================================
            // REGISTRO DE ROLES
            // ==========================================

            foreach (var rol in roles)
            {
                // Comprobar si el rol ya existe.
                if (!await roleManager.RoleExistsAsync(rol.Nombre))
                {
                    var nuevoRol = new Rol
                    {
                        Name = rol.Nombre,
                        Descripcion = rol.Descripcion,
                        Estado = true,
                        FechaCreacion = DateTime.UtcNow
                    };

                    // Crear el rol utilizando Identity.
                    var resultado =
                        await roleManager.CreateAsync(nuevoRol);

                    // Verificar si ocurrió algún error.
                    if (!resultado.Succeeded)
                    {
                        var errores = string.Join(
                            "; ",
                            resultado.Errors.Select(
                                e => e.Description)
                        );

                        throw new InvalidOperationException(
                            $"No se pudo crear el rol {rol.Nombre}: {errores}"
                        );
                    }
                }
            }
        }
    }
}