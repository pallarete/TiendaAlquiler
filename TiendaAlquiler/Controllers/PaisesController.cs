using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaAlquiler.Data;
using TiendaAlquiler.Models;


namespace TiendaAlquiler.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PaisesController : Controller
    {
        private readonly TiendaAlquilerDBContext _context;

        public PaisesController(TiendaAlquilerDBContext context)
        {
            _context = context;
        }

<<<<<<< HEAD
        // GET: Lista de Paises
=======
        // GET: Indice de Paises
>>>>>>> 723036fc6c72f1f1efc89c67b4c29c37151db85a
        public async Task<IActionResult> Index()
        {
            var paisesOrdenados = await _context.Paises
                .OrderBy(c => c.Nombre)
                .ToListAsync();
            return View(paisesOrdenados);
        }

<<<<<<< HEAD
        // GET: Paises
=======
        // GET: Paises Detalles
>>>>>>> 723036fc6c72f1f1efc89c67b4c29c37151db85a
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var pais = await _context.Paises
                .FirstOrDefaultAsync(m => m.PaisId == id);
            if (pais == null)
            {
                return NotFound();
            }
            return View(pais);
        }

<<<<<<< HEAD
        // GET: Crear Pais
=======
        // GET: Crear Paises
>>>>>>> 723036fc6c72f1f1efc89c67b4c29c37151db85a
        public IActionResult Create()
        {
            return View();
        }

<<<<<<< HEAD
        // POST: Crear Pais
=======
        // POST: Crear Paises
>>>>>>> 723036fc6c72f1f1efc89c67b4c29c37151db85a
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaisId,Nombre")] Pais pais)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pais);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pais);
        }

<<<<<<< HEAD
        // GET: Editar Pais
=======
        // GET: Editar Paises
>>>>>>> 723036fc6c72f1f1efc89c67b4c29c37151db85a
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pais = await _context.Paises.FindAsync(id);
            if (pais == null)
            {
                return NotFound();
            }
            return View(pais);
        }

<<<<<<< HEAD
        // POST: Editar Pais
=======
        // POST: Editar Paises
>>>>>>> 723036fc6c72f1f1efc89c67b4c29c37151db85a
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaisId,Nombre")] Pais pais)
        {
            if (id != pais.PaisId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pais);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaisFabricacionExists(pais.PaisId))
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
            return View(pais);
        }

<<<<<<< HEAD
        // GET: Borrar Pais
=======
        // GET: Borrar Paises
>>>>>>> 723036fc6c72f1f1efc89c67b4c29c37151db85a
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pais = await _context.Paises
                .FirstOrDefaultAsync(m => m.PaisId == id);
            if (pais == null)
            {
                return NotFound();
            }

            return View(pais);
        }

<<<<<<< HEAD
        // POST: Borrar Pais
=======
        // POST: Borrar Paises
>>>>>>> 723036fc6c72f1f1efc89c67b4c29c37151db85a
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cochesAsociados = await _context.Coches.AnyAsync(c => c.PaisId == id);

            if (cochesAsociados)
            {
                TempData["ErrorMessage"] = "No se puede borrar este país porque hay coches asociados a él. Por favor, cree un nuevo país o modifíquelo";
                return RedirectToAction("Delete", new { id });
            }
            var pais = await _context.Paises.FindAsync(id);
            if (pais != null)
            {
                _context.Paises.Remove(pais);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PaisFabricacionExists(int id)
        {
            return _context.Paises.Any(e => e.PaisId == id);
        }
    }
}
