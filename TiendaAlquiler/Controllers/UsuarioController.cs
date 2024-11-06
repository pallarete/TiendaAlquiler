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

        public UsuarioController(TiendaAlquilerDBContext context, UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
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

            var model = new UsuarioViewModel
            {
                Id = usuario.Id ?? "",  // Asegúrate de que no sea nulo
                UsuarioNombre = usuario.UsuarioNombre ?? "",
                Rol = usuario.Rol ?? "",  // Si Rol es de tipo string, puedes usar el operador null-coalescente
                UserName = usuario.UserName ?? "",
                Email = usuario.Email ?? "" // Lo mismo con Email si es opcional
            };

            return View(model);
        }

        // GET: Usuario/Create
        public IActionResult Create()
        {
            return View();
        }



        // POST: Usuario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioNombre,Rol,Email,Password,UserName")] UsuarioViewModel model)
        {
            if (ModelState.IsValid)
            {
                //mapeamos los datos del view model al modelo de base de datos
                var usuario = new Usuario
                {
                    UsuarioNombre = model.UsuarioNombre,//Asiganamos nombre desde el viewModel
                    Rol = model.Rol,
                    UserName = model.UserName,
                    Email = model.Email,
                    //SePueden asignar propiedades segun sea necesario
                };

                // Se recomienda en este caso usar UserManager para crear un usuario
                var result = await _userManager.CreateAsync(usuario, model.Password);
                if (result.Succeeded)
                {
                    // Si la creación es exitosa, redirigimos a la lista de usuarios
                    return RedirectToAction(nameof(Index));
                }

                // Si hubo algún error, mostramos los errores en el modelo
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
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
        public async Task<IActionResult> Edit(string id, [Bind("UsuarioNombre,Rol,Email,UserName")] UsuarioViewModel model)
        {
            if (id == null || !ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            //Actualizamos solo las propiedades permitidas
            usuario.UsuarioNombre = model.UsuarioNombre;
            usuario.Rol = model.Rol;
            usuario.UserName = model.UserName;
            usuario.Email = model.Email;

            var result = await _userManager.UpdateAsync(usuario);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            //agregamos errores al modelSatate si al actualizacion falla
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }




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

            var model = new UsuarioViewModel
            {
                Id = usuario.Id ?? "",  // Usamos ?? para asegurarnos de que no sea null
                UsuarioNombre = usuario.UsuarioNombre ?? "",  // Usamos ?? si la propiedad podría ser null
                Rol = usuario.Rol ?? "",
                UserName = usuario.UserName ?? "",
                Email = usuario.Email ?? ""
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
                var usuario = new Usuario
                {
                    UsuarioNombre = model.UsuarioNombre,
                    UserName = model.Email,
                    Email = model.Email,
                };
                var totalUsuarios = await _userManager.Users.CountAsync();

                usuario.Rol = totalUsuarios == 0 ? "Admin" : "User";

                var result = await _userManager.CreateAsync(usuario, model.Password);

                if (result.Succeeded)
                {
                    var addRoleResult = await _userManager.AddToRoleAsync(usuario, usuario.Rol);

                    if (!addRoleResult.Succeeded)
                    {
                        //manejamos los errores
                        foreach (var error in addRoleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    else
                    {
                        //Redirigimos al login si el registro es exitoso
                        return RedirectToAction("Login", "Usuario");
                    }

                }

                //Mostrara errores si la creacion falla
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

                if (usuario != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(usuario, model.Password, false, false);

                    if (result.Succeeded)
                    {

                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            return View(model);
        }


        private bool UsuarioExists(string id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
