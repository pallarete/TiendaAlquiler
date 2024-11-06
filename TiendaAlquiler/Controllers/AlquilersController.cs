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
        public async Task<IActionResult> CreateAsync()
        {
            ViewData["CocheId"] = new SelectList(_context.Coches, "CocheId", "Marca");
            ViewData["UsuarioId"] = new SelectList(await _userManager.Users.ToListAsync(), "Id", "Rol");

            return View();
        }

        // POST: Alquilers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlquilerId,CocheId,UsuarioId,FechaAlquiler,FechaDevolucion,PrecioFinal")] Alquiler alquiler)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alquiler);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CocheId"] = new SelectList(_context.Coches, "CocheId", "Marca", alquiler.CocheId);
            ViewData["UsuarioId"] = new SelectList(await _userManager.Users.ToListAsync(), "Id", "Rol");

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
