using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // GET: Usuario
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }

        // GET: Usuario/Details/5
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
                Id = usuario.Id ?? "",  // Aseg�rate de que no sea nulo
                UserName = usuario.UserName ?? "",
                Email = usuario.Email ?? "", // Lo mismo con Email si es opcional
            };

            return View(model);
        }

        // GET: Usuario/Edit/5
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

        // POST: Usuario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]





        // GET: Usuario/Delete/5
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
                Id = usuario.Id ?? "",  // Usamos ?? para asegurarnos de que no sea null
                UserName = usuario.UserName ?? "",
                Email = usuario.Email ?? "",

            };

            return View(model);
        }

        // POST: Usuario/Delete/5
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

        //GET: Usuario/Registro
        public IActionResult Register()
        {
            return View();
        }

        //POST: Usuario/Registro
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
                    // Verificar si es el primer usuario
                    if (!await _userManager.Users.AnyAsync()) // Si es el primer usuario
                    {
                        // Verificar si el rol Admin existe, si no, crear el rol
                        var roleExists = await _roleManager.RoleExistsAsync("Admin");
                        if (!roleExists)
                        {
                            await _roleManager.CreateAsync(new IdentityRole("Admin"));
                        }

                        // Asignar el rol de Admin al primer usuario
                        await _userManager.AddToRoleAsync(usuario, "Admin");
                    }

                    // Iniciar sesi�n autom�ticamente
                    await _signInManager.SignInAsync(usuario, isPersistent: false);
                    return RedirectToAction("Index", "Home"); // Redirigir despu�s de registro
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);

        }

        // GET: Usuario/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Usuario/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("UsuarioNombre,Password")] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByNameAsync(model.UsuarioNombre);

                if (usuario == null)
                {
                    ModelState.AddModelError(string.Empty, "El usuario no existe");

                }
                else

                {
                    var result = await _signInManager.PasswordSignInAsync(usuario, model.Password, false, false);

                    if (result.Succeeded)
                    {
                        // Redirigir dependiendo del rol, por ejemplo
                        if (await _userManager.IsInRoleAsync(usuario, "Admin"))
                        {
                            return RedirectToAction("Index", "Admin"); // o lo que sea tu p�gina de Admin
                        }
                        return RedirectToAction("Index", "Home");

                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "La contrase�a es incorrecta");

                    }
                }
            }
            return View(model);
        }

        //LOGOUT
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); //AQUI CIERRO LA SESION
            return RedirectToAction("Index", "Home");//REDIRIGO AL PRINCIPIO
        }


        private bool UsuarioExists(string id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
