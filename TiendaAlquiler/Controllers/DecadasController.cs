using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaAlquiler.Data;
using TiendaAlquiler.Models;


namespace TiendaAlquiler.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DecadasController : Controller
    {
        private readonly TiendaAlquilerDBContext _context;
        public DecadasController(TiendaAlquilerDBContext context)
        {
            _context = context;
        }

        // GET: Indice de Decadas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Decada.ToListAsync());
        }

        // GET: Detalles Decadas
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var decada = await _context.Decada
                .FirstOrDefaultAsync(m => m.DecadaId == id);
            if (decada == null)
            {
                return NotFound();
            }

            return View(decada);
        }

        // GET: Crear una nueva decada (Vista)
        public IActionResult Create()
        {
            return View();
        }

        // POST: Crear una nueva decada (Vista) Confirmar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DecadaId,AnioInicio")] Decada decada)
        {
            if (ModelState.IsValid)
            {
                _context.Add(decada);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(decada);
        }

        // GET: Editar una Decada (Vista)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var decada = await _context.Decada.FindAsync(id);
            if (decada == null)
            {
                return NotFound();
            }
            return View(decada);
        }

        // POST: Editar una Decada (Vista) Confirmar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DecadaId,AnioInicio")] Decada decada)
        {
            if (id != decada.DecadaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(decada);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DecadaExists(decada.DecadaId))
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
            return View(decada);
        }

        // GET: Borrar una decada (Vista)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var decada = await _context.Decada
                .FirstOrDefaultAsync(m => m.DecadaId == id);
            if (decada == null)
            {
                return NotFound();
            }

            return View(decada);
        }

        // POST: Borrar una decada (Vista) Confirmacion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cochesAsociados = await _context.Coches.AnyAsync(c => c.DecadaId == id);

            if (cochesAsociados)
            {
                //si la decada esta asociada a algun coche o a vario, me da igual
                TempData["ErrorMessage"] = "No se puede eliminar esta década porque está asociada a uno o varios coches.Por favor, modifíquela o cree una nueva.";
                return RedirectToAction("Delete", new { id });
            }

            //Si no hay coches asociados a esa carroceria la elimino
            var decada = await _context.Decada.FindAsync(id);
            if (decada != null)
            {
                _context.Decada.Remove(decada);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private bool DecadaExists(int id)
        {
            return _context.Decada.Any(e => e.DecadaId == id);
        }
    }
}
