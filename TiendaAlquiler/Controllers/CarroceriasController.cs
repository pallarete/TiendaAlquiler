using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TiendaAlquiler.Models;
using TiendaAlquiler.Data;
using Microsoft.AspNetCore.Authorization;


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

        // GET: Carrocerias
        public async Task<IActionResult> Index()
        {
            var carroceriasOrdenadas = await _context.Carroceria
                .OrderBy(c => c.Tipo)
                .ToListAsync();
            return View(carroceriasOrdenadas);
        }

        // GET: Carrocerias/Details/5
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

        // GET: Carrocerias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Carrocerias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Carrocerias/Edit/5
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

        // POST: Carrocerias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Carrocerias/Delete/5
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

        // POST: Carrocerias/Delete/5
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
