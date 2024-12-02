using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaAlquiler.Data;
using TiendaAlquiler.Models;

namespace TiendaAlquiler.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ColorsController(TiendaAlquilerDBContext context) : Controller
    {
        private readonly TiendaAlquilerDBContext _context = context;

        // GET:Lista de Lso colores
        public async Task<IActionResult> Index()
        {
            var coloresOrdenados = await _context.Colors
                 .OrderBy(c => c.Nombre)
                 .ToListAsync();
            return View(coloresOrdenados);
        }

        // GET: Detalles De cada color
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var color = await _context.Colors
                .FirstOrDefaultAsync(m => m.ColorId == id);
            if (color == null)
            {
                return NotFound();
            }

            return View(color);
        }

        // GET: Crear un nuevo color (Vista)
        public IActionResult Create()
        {
            return View();
        }

        // POST: Crear un nuevo color

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ColorId,Nombre")] Color color)
        {
            if (ModelState.IsValid)
            {
                _context.Add(color);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(color);
        }

        // GET: Editar color (Vista)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var color = await _context.Colors.FindAsync(id);
            if (color == null)
            {
                return NotFound();
            }
            return View(color);
        }

        // POST: Editar color (Vista)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ColorId,Nombre")] Color color)
        {
            if (id != color.ColorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(color);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ColorExists(color.ColorId))
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
            return View(color);
        }

        // GET: Borra color (Vista)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var color = await _context.Colors
                .FirstOrDefaultAsync(m => m.ColorId == id);
            if (color == null)
            {
                return NotFound();
            }

            return View(color);
        }

        // POST: Borra color (Vista)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cochesAsociados = await _context.Coches.AnyAsync(c => c.ColorId == id);
            if (cochesAsociados)
            {
                //Envio el mensaje a TempData
                TempData["ErrorMessage"] = "No se puede borrar este color porque hay coches asociados a él. Por favor, modifíquelo o cree un color nuevo.";
                return RedirectToAction("Delete", new { id });
            }

            //eloimino el Color si no hay coches asociados
            var color = await _context.Colors.FindAsync(id);
            if (color != null)
            {
                _context.Colors.Remove(color);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ColorExists(int id)
        {
            return _context.Colors.Any(e => e.ColorId == id);
        }
    }
}
