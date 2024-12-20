using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TiendaAlquiler.Data;
using TiendaAlquiler.Models;

var builder = WebApplication.CreateBuilder(args);
// Configurar logging para la consola
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug); // Opcional

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<TiendaAlquilerDBContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionSqlServer")));


builder.Services.AddIdentity<Usuario, IdentityRole>(options =>
   options.SignIn.RequireConfirmedAccount = false)
.AddEntityFrameworkStores<TiendaAlquilerDBContext>()
.AddDefaultTokenProviders();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await CreateRolesAsync(app.Services);

await app.RunAsync();

static async Task CreateRolesAsync(IServiceProvider services)
{
   
    using var scope = services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = ["Admin", "User"];

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

}