using GestorDeClubes.Business.Models;
using GestorDeClubes.Data.Entities;

namespace GestorDeClubes.Business.Interfaces
{
    public interface IRolService
    {
        Task<List<Rol>> ObtenerTodosAsync();

        Task<Rol?> ObtenerPorIdAsync(int id);

        Task<ResultadoOperacion> CrearAsync(
            string nombre,
            string descripcion);

        Task<ResultadoOperacion> EditarAsync(
            int id,
            string nombre,
            string descripcion);

        Task<ResultadoOperacion> CambiarEstadoAsync(int id);

        Task<ResultadoOperacion> EliminarAsync(int id);

        bool EsRolBase(string? nombre);
    }
}