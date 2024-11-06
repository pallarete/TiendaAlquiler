using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaAlquiler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TiendaAlquiler.Data;


namespace TiendaAlquiler.Controllers
{
    public class CochesController : Controller
    {
        private readonly TiendaAlquilerDBContext _context;

        public CochesController(TiendaAlquilerDBContext context)
        {
            _context = context;
        }

        // GET: Coches
        public async Task<IActionResult> Index()
        {
            var tiendaAlquilerDBContext = _context.Coches.Include(c => c.Carroceria).Include(c => c.Color).Include(c => c.Decada).Include(c => c.PaisFabricacion);
            return View(await tiendaAlquilerDBContext.ToListAsync());
        }

        // GET: Coches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coche = await _context.Coches
                .Include(c => c.Carroceria)
                .Include(c => c.Color)
                .Include(c => c.Decada)
                .Include(c => c.PaisFabricacion)
                .Include(c => c.Fotos)
                .FirstOrDefaultAsync(m => m.CocheId == id);
            if (coche == null)
            {
                return NotFound();
            }

            return View(coche);
        }

        // GET: Coches/Create
        public IActionResult Create()
        {
            ViewData["CarroceriaId"] = new SelectList(_context.Carroceria, "CarroceriaId", "Tipo");
            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "Nombre");
            ViewData["DecadaId"] = new SelectList(_context.Decada, "DecadaId", "DecadaId");
            ViewData["PaisFabricacionId"] = new SelectList(_context.PaisFabricacions, "PaisFabricacionId", "Nombre");
            return View();
        }

        // POST: Coches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CocheId,Marca,Modelo,AnioFabricacion,PrecioAlquiler,EstaAlquilado,ColorId,CarroceriaId,DecadaId,PaisFabricacionId")] Coche coche)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coche);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarroceriaId"] = new SelectList(_context.Carroceria, "CarroceriaId", "Tipo", coche.CarroceriaId);
            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "Nombre", coche.ColorId);
            ViewData["DecadaId"] = new SelectList(_context.Decada, "DecadaId", "DecadaId", coche.DecadaId);
            ViewData["PaisFabricacionId"] = new SelectList(_context.PaisFabricacions, "PaisFabricacionId", "Nombre", coche.PaisFabricacionId);
            return View(coche);
        }

        // GET: Coches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coche = await _context.Coches.FindAsync(id);
            if (coche == null)
            {
                return NotFound();
            }
            ViewData["CarroceriaId"] = new SelectList(_context.Carroceria, "CarroceriaId", "Tipo", coche.CarroceriaId);
            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "Nombre", coche.ColorId);
            ViewData["DecadaId"] = new SelectList(_context.Decada, "DecadaId", "DecadaId", coche.DecadaId);
            ViewData["PaisFabricacionId"] = new SelectList(_context.PaisFabricacions, "PaisFabricacionId", "Nombre", coche.PaisFabricacionId);
            return View(coche);
        }

        // POST: Coches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CocheId,Marca,Modelo,AnioFabricacion,PrecioAlquiler,EstaAlquilado,ColorId,CarroceriaId,DecadaId,PaisFabricacionId")] Coche coche)
        {
            if (id != coche.CocheId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coche);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CocheExists(coche.CocheId))
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
            ViewData["CarroceriaId"] = new SelectList(_context.Carroceria, "CarroceriaId", "Tipo", coche.CarroceriaId);
            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "Nombre", coche.ColorId);
            ViewData["DecadaId"] = new SelectList(_context.Decada, "DecadaId", "DecadaId", coche.DecadaId);
            ViewData["PaisFabricacionId"] = new SelectList(_context.PaisFabricacions, "PaisFabricacionId", "Nombre", coche.PaisFabricacionId);
            return View(coche);
        }

        // GET: Coches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coche = await _context.Coches
                .Include(c => c.Carroceria)
                .Include(c => c.Color)
                .Include(c => c.Decada)
                .Include(c => c.PaisFabricacion)
                .FirstOrDefaultAsync(m => m.CocheId == id);
            if (coche == null)
            {
                return NotFound();
            }

            return View(coche);
        }

        // POST: Coches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coche = await _context.Coches.FindAsync(id);
            if (coche != null)
            {
                _context.Coches.Remove(coche);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CocheExists(int id)
        {
            return _context.Coches.Any(e => e.CocheId == id);
        }
    }
}
