using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TiendaAlquiler.Models;
using TiendaAlquiler.Data;


namespace TiendaAlquiler.Controllers
{
    public class AlquilersController : Controller
    {
        private readonly TiendaAlquilerDBContext _context;
        private readonly UserManager<Usuario> _userManager;

        public AlquilersController(TiendaAlquilerDBContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Alquilers
        public async Task<IActionResult> Index()
        {
            var tiendaAlquilerDBContext = _context.Alquilers.Include(a => a.Coche).Include(a => a.Usuario);
            return View(await tiendaAlquilerDBContext.ToListAsync());
        }

        // GET: Alquilers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alquiler = await _context.Alquilers
                .Include(a => a.Coche)
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(m => m.AlquilerId == id);
            if (alquiler == null)
            {
                return NotFound();
            }

            return View(alquiler);
        }

        // GET: Alquilers/Create
        [HttpGet]
        public async Task<IActionResult> Create(int cocheId, string usuarioId)
        {

            // Verificar si el coche y el usuario existen
            var coche = await _context.Coches.FindAsync(cocheId);
            var usuario = await _userManager.FindByIdAsync(usuarioId);

            if (coche == null || usuario == null)
            {
                return NotFound();
            }

            // Crear una instancia de Alquiler prellenada con el coche y el usuario seleccionados
            var alquiler = new Alquiler
            {
                CocheId = cocheId,
                UsuarioId = usuarioId,
            };

            ViewData["CocheId"] = new SelectList(_context.Coches, "CocheId", "Marca", alquiler.CocheId);
            ViewData["UsuarioId"] = new SelectList(await _userManager.Users.ToListAsync(), "Id", "UserName", alquiler.UsuarioId);

            return View(alquiler);

        }

        // POST: Alquilers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlquilerId,CocheId,UsuarioId,FechaAlquiler,FechaDevolucion")] Alquiler alquiler)
        {
            if (ModelState.IsValid)
            {
                // Obtener el coche desde la base de datos utilizando el CocheId
                var coche = await _context.Coches.FindAsync(alquiler.CocheId);


                //Si el coche no fue encontrado
                if (coche == null)
                {

                    ModelState.AddModelError("", "El coche no existe");
                    return View(alquiler);
                }

                // Convertir las fechas de DateOnly a DateTime para poder realizar la operación de días
                DateTime fechaAlquiler = alquiler.FechaAlquiler.ToDateTime(new TimeOnly());
                DateTime fechaDevolucion = alquiler.FechaDevolucion.ToDateTime(new TimeOnly());

                // Validar que la fecha de devolución es posterior a la fecha de alquiler
                if (fechaDevolucion <= fechaAlquiler)
                {
                    ModelState.AddModelError("", "La fecha de devolución debe ser posterior a la fecha de alquiler.");
                    return View(alquiler);

                }

                // Verificar si ya existe un alquiler para el mismo coche en el rango de fechas
                var alquileresCoche = await _context.Alquilers
                    .Where(a => a.CocheId == alquiler.CocheId)
                    .ToListAsync();

                bool fechasSolapadas = alquileresCoche
                    .Any(a =>
                        (fechaAlquiler >= a.FechaAlquiler.ToDateTime(new TimeOnly()) && fechaAlquiler < a.FechaDevolucion.ToDateTime(new TimeOnly())) ||
                        (fechaDevolucion > a.FechaAlquiler.ToDateTime(new TimeOnly()) && fechaDevolucion <= a.FechaDevolucion.ToDateTime(new TimeOnly())) ||
                        (fechaAlquiler <= a.FechaAlquiler.ToDateTime(new TimeOnly()) && fechaDevolucion >= a.FechaDevolucion.ToDateTime(new TimeOnly()))
                    );

                if (fechasSolapadas)
                {
                    ModelState.AddModelError("", "Este coche ya está alquilado en el rango de fechas seleccionado.");
                    return View(alquiler);
                }


                //Calculo el numero de dias de alquiler
                //Calculo el precio final del Alquiler
                var diasAlquiler = (fechaDevolucion - fechaAlquiler).Days;
                alquiler.PrecioFinal = coche.PrecioAlquiler * diasAlquiler;

                // Guardar el alquiler en la base de datos
                _context.Add(alquiler);
                await _context.SaveChangesAsync();

                //Redirigo a la lista de alquileres o donde quiera
                return RedirectToAction(nameof(Index));

            }

            // Si el modelo no es válido, devolver las listas de coches y usuarios para la vista
            ViewData["CocheId"] = new SelectList(_context.Coches, "CocheId", "Marca", alquiler.CocheId);
            ViewData["UsuarioId"] = new SelectList(await _userManager.Users.ToListAsync(), "Id", "UserName", alquiler.UsuarioId);

            return View(alquiler);

        }





        // GET: Alquilers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alquiler = await _context.Alquilers.FindAsync(id);
            if (alquiler == null)
            {
                return NotFound();
            }
            ViewData["CocheId"] = new SelectList(_context.Coches, "CocheId", "Marca", alquiler.CocheId);
            ViewData["UsuarioId"] = new SelectList(await _userManager.Users.ToListAsync(), "Id", "Rol");


            return View(alquiler);
        }

        // POST: Alquilers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlquilerId,CocheId,UsuarioId,FechaAlquiler,FechaDevolucion,PrecioFinal")] Alquiler alquiler)
        {
            if (id != alquiler.AlquilerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alquiler);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlquilerExists(alquiler.AlquilerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CocheId"] = new SelectList(_context.Coches, "CocheId", "Marca", alquiler.CocheId);
            ViewData["UsuarioId"] = new SelectList(await _userManager.Users.ToListAsync(), "Id", "Rol");

            return View(alquiler);
        }

        // GET: Alquilers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alquiler = await _context.Alquilers
                .Include(a => a.Coche)
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(m => m.AlquilerId == id);
            if (alquiler == null)
            {
                return NotFound();
            }

            return View(alquiler);
        }

        // POST: Alquilers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alquiler = await _context.Alquilers.FindAsync(id);
            if (alquiler != null)
            {
                _context.Alquilers.Remove(alquiler);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlquilerExists(int id)
        {
            return _context.Alquilers.Any(e => e.AlquilerId == id);
        }
    }
}
