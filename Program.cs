using Flash_Lector.Data;
using Flash_Lector.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Conexión con SQL Server.
builder.Services.AddDbContext<ContextBaseDatos>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/InicioSesion";
        options.AccessDeniedPath = "/Cuenta/AccesoDenegado";

        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;//esta línea asegura que la cookie solo se envíe a través de conexiones HTTPS.
        options.Cookie.SameSite = SameSiteMode.Lax;//esta línea permite que la cookie se envíe en solicitudes de diferentes sitios, lo cual es útil para aplicaciones que pueden estar alojadas en diferentes dominios o subdominios.
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); //esta línea establece el tiempo de expiración de la cookie en 30 minutos, lo que significa que si el usuario no realiza ninguna acción durante ese tiempo, la cookie expirará y el usuario tendrá que iniciar sesión nuevamente.
        options.SlidingExpiration = true; //esta línea permite que la expiración de la cookie se desplace hacia adelante cada vez que el usuario realiza una solicitud, lo que significa que si el usuario está activo, la cookie se mantendrá válida y no expirará mientras siga interactuando con la aplicación.
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Sin esto, los archivos .epub y .cbz subidos devolverían 404 (extensión desconocida).
var tiposContenido = new FileExtensionContentTypeProvider();
tiposContenido.Mappings[".epub"] = "application/epub+zip";
tiposContenido.Mappings[".cbz"] = "application/vnd.comicbook+zip";

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = tiposContenido });
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Crea los usuarios de prueba y el catálogo inicial
using (var alcance = app.Services.CreateScope())
    await InicializadorBaseDatos.InicializarAsync(alcance.ServiceProvider);

app.Run();