using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaAlquiler.Data;
using TiendaAlquiler.Models;
using TiendaAlquiler.ViewModels;

namespace TiendaAlquiler.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly TiendaAlquilerDBContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuarioController(TiendaAlquilerDBContext context, UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        
        // Este metodo no se usa
        // GET: Usuario Indice
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }

        // Este metodo no se usa
        // GET: Usuario Detalles
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var model = new Usuario
            {
                Id = usuario.Id ?? "",  // Me aseguro que existe
                UserName = usuario.UserName ?? "",
                Email = usuario.Email ?? "", // el email tambien
            };

            return View(model);
        }

        // Este metodo no se usa
        // GET: Editar usuario
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // Este metodo no se usa
        // GET: Borrar Usuario
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var model = new Usuario
            {
                Id = usuario.Id ?? "",  
                UserName = usuario.UserName ?? "",
                Email = usuario.Email ?? "",

            };

            return View(model);
        }

        // Este metodo no se usa
        // POST: Borrar Usuario
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario != null)
            {
                var result = await _userManager.DeleteAsync(usuario);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                //Si falla el borrado agregamos  errores
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return RedirectToAction(nameof(Index));
        }

        //GET: Registrar usuario
        public IActionResult Register()
        {
            return View();
        }

        //POST: Registrar usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = new Usuario { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(usuario, model.Password);

                if (result.Succeeded)
                {
                    // Verifico si es el primer usuario
                    var usersCount = await _userManager.Users.CountAsync();

                    if (usersCount == 1) // Si es el primer usuario
                    {
                        // Verifico si el rol Admin existe, si no, se crea el rol porque el proigrama esta configurado
                        var roleExists = await _roleManager.RoleExistsAsync("Admin");
                        if (!roleExists)
                        {
                            await _roleManager.CreateAsync(new IdentityRole("Admin"));
                        }

                        // Se asigna el rol de Admin al primer usuario
                        await _userManager.AddToRoleAsync(usuario, "Admin");
                    }

                    // Iniciar sesión automáticamente
                    await _signInManager.SignInAsync(usuario, isPersistent: false);
                    return RedirectToAction("Index", "Home"); // Redirigir después de registro
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // GET: Loguear Usuario
        public IActionResult Login()
        {
            return View();
        }

        // POST:Loguear Usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("UsuarioNombre,Password")] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByNameAsync(model.UsuarioNombre);

                if (usuario == null)
                {
                    //Creo el menasje de error para el modal
                    TempData["ErrorMessage"] = "El usuario no está registrado. Por favor, regístrese para disfrutar de nuestros vehículos.";
                    return RedirectToAction("login");
                }
                else
                {
                    var result = await _signInManager.PasswordSignInAsync(usuario, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        // Redirigo dependiendo del rol
                        if (await _userManager.IsInRoleAsync(usuario, "Admin"))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "La contraseña es incorrecta");

                    }
                }
            }
            return View(model);
        }

        // Logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); //AQUI CIERRO LA SESION
            return RedirectToAction("Index", "Home");//REDIRIGO AL PRINCIPIO
        }

        private bool UsuarioExists(string id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
        // Acción para manejar el acceso denegado
        public IActionResult AccessDenied()
        {
            return View(); 
        }
    }

}
