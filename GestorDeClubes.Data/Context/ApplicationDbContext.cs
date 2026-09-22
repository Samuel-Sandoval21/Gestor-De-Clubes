
using GestorDeClubes.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestorDeClubes.Data.Context
{
    public class ApplicationDbContext
        : IdentityDbContext<Usuario, Rol, int>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ==========================================
        // HU-05: REGISTROS DE AUDITORÍA
        // ==========================================

        public DbSet<AuditoriaAcceso> AuditoriaAccesos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ==========================================
            // TABLA: USUARIOS
            // ==========================================

            builder.Entity<Usuario>(entity =>
            {
                entity.ToTable("USUARIOS");

                entity.Property(u => u.Id)
                    .HasColumnName("UsuarioID");

                entity.Property(u => u.Email)
                    .HasColumnName("CorreoInstitucional")
                    .HasMaxLength(256);

                entity.Property(u => u.Nombre)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(u => u.Apellidos)
                    .HasMaxLength(150)
                    .IsRequired();

                // Evitar correos institucionales duplicados.
                // Identity utiliza NormalizedEmail para las búsquedas.
                entity.HasIndex(u => u.NormalizedEmail)
                    .IsUnique();
            });

            // ==========================================
            // TABLA: ROLES
            // ==========================================

            builder.Entity<Rol>(entity =>
            {
                entity.ToTable("ROLES");

                entity.Property(r => r.Id)
                    .HasColumnName("RolID");

                entity.Property(r => r.Name)
                    .HasColumnName("Nombre")
                    .HasMaxLength(256);

                entity.Property(r => r.Descripcion)
                    .HasMaxLength(250);
            });

            // ==========================================
            // HU-05: TABLA AUDITORIA_ACCESOS
            // ==========================================

            builder.Entity<AuditoriaAcceso>(entity =>
            {
                entity.ToTable("AUDITORIA_ACCESOS");

                entity.HasKey(a => a.AuditoriaAccesoID);

                entity.Property(a => a.AuditoriaAccesoID)
                    .ValueGeneratedOnAdd();

                entity.Property(a => a.UsuarioID)
                    .IsRequired();

                entity.Property(a => a.TipoEvento)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(a => a.FechaHora)
                    .IsRequired();

                // Cada registro pertenece a un usuario.
                // Restringimos el borrado en cascada para
                // conservar la integridad de la bitácora.
                entity.HasOne(a => a.Usuario)
                    .WithMany()
                    .HasForeignKey(a => a.UsuarioID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Facilita la consulta cronológica.
                entity.HasIndex(a => a.FechaHora);
            });

            // ==========================================
            // TABLAS ADICIONALES DE IDENTITY
            // ==========================================

            // Relación entre usuarios y roles
            builder.Entity<IdentityUserRole<int>>()
                .ToTable("USUARIOS_ROLES");

            // Claims de los usuarios
            builder.Entity<IdentityUserClaim<int>>()
                .ToTable("USUARIOS_CLAIMS");

            // Proveedores de inicio de sesión
            builder.Entity<IdentityUserLogin<int>>()
                .ToTable("USUARIOS_LOGINS");

            // Tokens de autenticación
            builder.Entity<IdentityUserToken<int>>()
                .ToTable("USUARIOS_TOKENS");

            // Claims de los roles
            builder.Entity<IdentityRoleClaim<int>>()
                .ToTable("ROLES_CLAIMS");
        }
    }
}