using GestorDeClubes.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace GestorDeClubes.Repository.Interfaces
{
    public interface IRolRepository
    {
        Task<List<Rol>> ObtenerTodosAsync();

        Task<Rol?> ObtenerPorIdAsync(int id);

        Task<Rol?> ObtenerPorNombreAsync(string nombre);

        Task<bool> EstaAsignadoAUsuariosAsync(int rolId);

        Task<IdentityResult> CrearAsync(Rol rol);

        Task<IdentityResult> ActualizarAsync(Rol rol);
    }
}