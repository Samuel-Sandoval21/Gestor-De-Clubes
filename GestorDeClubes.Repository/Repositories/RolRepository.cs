using GestorDeClubes.Data.Context;
using GestorDeClubes.Data.Entities;
using GestorDeClubes.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace GestorDeClubes.Repository.Repositories
{
    public class RolRepository : IRolRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<Rol> _roleManager;

        public RolRepository(
            ApplicationDbContext context,
            RoleManager<Rol> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public RolRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Rol>> ObtenerTodosAsync()
        {
            return await _context.Roles
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<Rol?> ObtenerPorIdAsync(int id)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Rol?> ObtenerPorNombreAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return null;
            }

            string nombreNormalizado = nombre.Trim().ToUpperInvariant();

            return await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r =>
                    r.NormalizedName == nombreNormalizado);
        }

        public async Task<bool> EstaAsignadoAUsuariosAsync(int rolId)
        {
            return await _context.UserRoles
                .AsNoTracking()
                .AnyAsync(ur => ur.RoleId == rolId);
        }

        public async Task<IdentityResult> CrearAsync(Rol rol)
        {
            return await _roleManager.CreateAsync(rol);
        }

        public async Task<IdentityResult> ActualizarAsync(Rol rol)
        {
            return await _roleManager.UpdateAsync(rol);
        }
    }
}