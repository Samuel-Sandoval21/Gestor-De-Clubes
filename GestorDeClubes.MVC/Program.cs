
using GestorDeClubes.Data.Context;
using GestorDeClubes.Data.Entities;
using GestorDeClubes.Data.Seed;
using GestorDeClubes.MVC.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// SERVICIOS MVC
// ==========================================

builder.Services.AddControllersWithViews();

// ==========================================
// ENTITY FRAMEWORK CORE - SQL SERVER
// ==========================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ==========================================
// ASP.NET CORE IDENTITY
// ==========================================

builder.Services.AddIdentity<Usuario, Rol>(options =>
{
    // Requisitos de contraseña
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Bloqueo después de cinco intentos fallidos
    options.Lockout.DefaultLockoutTimeSpan =
        TimeSpan.FromMinutes(15);

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Correo electrónico único
    options.User.RequireUniqueEmail = true;

    // Activaremos la confirmación cuando implementemos
    // la verificación del correo institucional.
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ==========================================
// COOKIE DE AUTENTICACIÓN
// ==========================================

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Cuenta/Login";
    options.LogoutPath = "/Cuenta/Logout";
    options.AccessDeniedPath = "/Cuenta/AccesoDenegado";

    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});

var app = builder.Build();

// ==========================================
// CONFIGURACIÓN DEL ENTORNO
// ==========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Autenticación antes de autorización
app.UseAuthentication();

app.UseAuthorization();

// ==========================================
// INICIALIZACIÓN DE IDENTITY
// ==========================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Crear los cuatro roles si todavía no existen.
    await IdentitySeeder.InicializarRolesAsync(services);

    // Crear una cuenta ficticia únicamente en Development.
    if (app.Environment.IsDevelopment())
    {
        await UsuarioPruebaSeeder.InicializarAsync(services);
    }
}

// ==========================================
// RUTA MVC PREDETERMINADA
// ==========================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();