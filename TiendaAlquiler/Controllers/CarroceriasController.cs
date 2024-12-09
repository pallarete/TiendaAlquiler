using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaAlquiler.Data;
using TiendaAlquiler.Models;


namespace TiendaAlquiler.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CarroceriasController : Controller
    {
        private readonly TiendaAlquilerDBContext _context;
        public CarroceriasController(TiendaAlquilerDBContext context)
        {
            _context = context;
        }

        // GET Indice Carrocerias (alfabeticamente)
        public async Task<IActionResult> Index()
        {
            var carroceriasOrdenadas = await _context.Carroceria
                .OrderBy(c => c.Tipo)
                .ToListAsync();
            return View(carroceriasOrdenadas);
        }

        // GET: Detalles Carrocerias
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var carroceria = await _context.Carroceria
                .FirstOrDefaultAsync(m => m.CarroceriaId == id);
            if (carroceria == null)
            {
                return NotFound();
            }
            return View(carroceria);
        }

        // GET: Crear Carrocerias
        public IActionResult Create()
        {
            return View();
        }

        // POST: Crear Carrocerias
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CarroceriaId,Tipo")] Carroceria carroceria)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carroceria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(carroceria);
        }

        // GET: Editar Carroceria
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var carroceria = await _context.Carroceria.FindAsync(id);
            if (carroceria == null)
            {
                return NotFound();
            }
            return View(carroceria);
        }

        // POST: Editar Carroceria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarroceriaId,Tipo")] Carroceria carroceria)
        {
            if (id != carroceria.CarroceriaId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carroceria);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarroceriaExists(carroceria.CarroceriaId))
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
            return View(carroceria);
        }

        // GET: Borrar Carroceria
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var carroceria = await _context.Carroceria
                .FirstOrDefaultAsync(m => m.CarroceriaId == id);
            if (carroceria == null)
            {
                return NotFound();
            }

            return View(carroceria);
        }

        // POST: Borrar Carroceria
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cochesAsociados = await _context.Coches.AnyAsync(c => c.CarroceriaId == id);
            if (cochesAsociados)
            {
                //Envio un mensaje a Tempdata
                TempData["ErrorMessage"] = "No se puede eliminar esta carrocería porque hay coches asociados a ella. Por favor, modifíquela o cree una nueva.";
                return RedirectToAction("Delete", new { id });
            }
            //Elimino la carroceria si no hay coches asociados
            var carroceria = await _context.Carroceria.FindAsync(id);
            if (carroceria != null)
            {
                _context.Carroceria.Remove(carroceria);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CarroceriaExists(int id)
        {
            return _context.Carroceria.Any(e => e.CarroceriaId == id);
        }
    }
}
