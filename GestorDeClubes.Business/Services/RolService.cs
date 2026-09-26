using GestorDeClubes.Business.Interfaces;
using GestorDeClubes.Business.Models;
using GestorDeClubes.Data.Entities;
using GestorDeClubes.Repository.Interfaces;

namespace GestorDeClubes.Business.Services
{
    public class RolService : IRolService
    {
        private readonly IRolRepository _rolRepository;

        private static readonly HashSet<string> RolesBase =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "Administrador",
                "Estudiante",
                "Asistente de Club",
                "Personal de Bienestar Estudiantil"
            };

        public RolService(IRolRepository rolRepository)
        {
            _rolRepository = rolRepository;
        }

        public async Task<List<Rol>> ObtenerTodosAsync()
        {
            return await _rolRepository.ObtenerTodosAsync();
        }

        public async Task<Rol?> ObtenerPorIdAsync(int id)
        {
            return await _rolRepository.ObtenerPorIdAsync(id);
        }

        public bool EsRolBase(string? nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return false;
            }

            return RolesBase.Contains(nombre.Trim());
        }

        public async Task<ResultadoOperacion> CrearAsync(
            string nombre,
            string descripcion)
        {
            nombre = nombre?.Trim() ?? string.Empty;
            descripcion = descripcion?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(descripcion))
            {
                return ResultadoOperacion.Error(
                    "Debe completar todos los campos obligatorios.");
            }

            var rolExistente =
                await _rolRepository.ObtenerPorNombreAsync(nombre);

            if (rolExistente != null)
            {
                return ResultadoOperacion.Error(
                    "El rol ya se encuentra registrado.");
            }

            var rol = new Rol
            {
                Name = nombre,
                Descripcion = descripcion,
                Estado = true,
                FechaCreacion = DateTime.UtcNow
            };

            var resultado =
                await _rolRepository.CrearAsync(rol);

            if (!resultado.Succeeded)
            {
                string errores = string.Join(
                    " ",
                    resultado.Errors.Select(e => e.Description));

                return ResultadoOperacion.Error(
                    string.IsNullOrWhiteSpace(errores)
                        ? "No fue posible crear el rol."
                        : errores);
            }

            return ResultadoOperacion.Ok(
                "El rol fue creado correctamente.");
        }

        public async Task<ResultadoOperacion> EditarAsync(
            int id,
            string nombre,
            string descripcion)
        {
            nombre = nombre?.Trim() ?? string.Empty;
            descripcion = descripcion?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(descripcion))
            {
                return ResultadoOperacion.Error(
                    "Debe completar todos los campos obligatorios.");
            }

            var rol =
                await _rolRepository.ObtenerPorIdAsync(id);

            if (rol == null)
            {
                return ResultadoOperacion.Error(
                    "El rol no fue encontrado.");
            }

            // Los roles base pueden modificar su descripción,
            // pero su nombre forma parte del contrato de autorización.
            if (EsRolBase(rol.Name) &&
                !string.Equals(
                    rol.Name,
                    nombre,
                    StringComparison.OrdinalIgnoreCase))
            {
                return ResultadoOperacion.Error(
                    "El nombre de un rol base no puede modificarse.");
            }

            var rolConMismoNombre =
                await _rolRepository.ObtenerPorNombreAsync(nombre);

            if (rolConMismoNombre != null &&
                rolConMismoNombre.Id != rol.Id)
            {
                return ResultadoOperacion.Error(
                    "Ya existe otro rol con ese nombre.");
            }

            rol.Name = nombre;
            rol.Descripcion = descripcion;
            rol.FechaModificacion = DateTime.UtcNow;

            var resultado =
                await _rolRepository.ActualizarAsync(rol);

            if (!resultado.Succeeded)
            {
                string errores = string.Join(
                    " ",
                    resultado.Errors.Select(e => e.Description));

                return ResultadoOperacion.Error(
                    string.IsNullOrWhiteSpace(errores)
                        ? "No fue posible actualizar el rol."
                        : errores);
            }

            return ResultadoOperacion.Ok(
                "El rol fue actualizado correctamente.");
        }

        public async Task<ResultadoOperacion> CambiarEstadoAsync(int id)
        {
            var rol =
                await _rolRepository.ObtenerPorIdAsync(id);

            if (rol == null)
            {
                return ResultadoOperacion.Error(
                    "El rol no fue encontrado.");
            }

            rol.Estado = !rol.Estado;
            rol.FechaModificacion = DateTime.UtcNow;

            var resultado =
                await _rolRepository.ActualizarAsync(rol);

            if (!resultado.Succeeded)
            {
                return ResultadoOperacion.Error(
                    "No fue posible actualizar el estado del rol.");
            }

            return ResultadoOperacion.Ok(
                rol.Estado
                    ? "El rol fue activado correctamente."
                    : "El rol fue desactivado correctamente.");
        }

        public async Task<ResultadoOperacion> EliminarAsync(int id)
        {
            var rol =
                await _rolRepository.ObtenerPorIdAsync(id);

            if (rol == null)
            {
                return ResultadoOperacion.Error(
                    "El rol no fue encontrado.");
            }

            if (EsRolBase(rol.Name))
            {
                return ResultadoOperacion.Error(
                    "Los roles base del sistema no pueden eliminarse.");
            }

            if (await _rolRepository.EstaAsignadoAUsuariosAsync(id))
            {
                return ResultadoOperacion.Error(
                    "El rol no puede eliminarse porque está asignado a uno o más usuarios.");
            }

            if (!rol.Estado)
            {
                return ResultadoOperacion.Error(
                    "El rol ya se encuentra eliminado.");
            }

            // Soft Delete.
            rol.Estado = false;
            rol.FechaModificacion = DateTime.UtcNow;

            var resultado =
                await _rolRepository.ActualizarAsync(rol);

            if (!resultado.Succeeded)
            {
                return ResultadoOperacion.Error(
                    "No fue posible eliminar el rol.");
            }

            return ResultadoOperacion.Ok(
                "El rol fue eliminado correctamente.");
        }
    }
}