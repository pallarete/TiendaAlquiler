using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TiendaAlquiler.Models;
using TiendaAlquiler.Data;


namespace TiendaAlquiler.Controllers
{
    public class PaisFabricacionsController : Controller
    {
        private readonly TiendaAlquilerDBContext _context;

        public PaisFabricacionsController(TiendaAlquilerDBContext context)
        {
            _context = context;
        }

        // GET: PaisFabricacions
        public async Task<IActionResult> Index()
        {
            return View(await _context.PaisFabricacions.ToListAsync());
        }

        // GET: PaisFabricacions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paisFabricacion = await _context.PaisFabricacions
                .FirstOrDefaultAsync(m => m.PaisFabricacionId == id);
            if (paisFabricacion == null)
            {
                return NotFound();
            }

            return View(paisFabricacion);
        }

        // GET: PaisFabricacions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PaisFabricacions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaisFabricacionId,Nombre")] PaisFabricacion paisFabricacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paisFabricacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(paisFabricacion);
        }

        // GET: PaisFabricacions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paisFabricacion = await _context.PaisFabricacions.FindAsync(id);
            if (paisFabricacion == null)
            {
                return NotFound();
            }
            return View(paisFabricacion);
        }

        // POST: PaisFabricacions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaisFabricacionId,Nombre")] PaisFabricacion paisFabricacion)
        {
            if (id != paisFabricacion.PaisFabricacionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paisFabricacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaisFabricacionExists(paisFabricacion.PaisFabricacionId))
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
            return View(paisFabricacion);
        }

        // GET: PaisFabricacions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paisFabricacion = await _context.PaisFabricacions
                .FirstOrDefaultAsync(m => m.PaisFabricacionId == id);
            if (paisFabricacion == null)
            {
                return NotFound();
            }

            return View(paisFabricacion);
        }

        // POST: PaisFabricacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paisFabricacion = await _context.PaisFabricacions.FindAsync(id);
            if (paisFabricacion != null)
            {
                _context.PaisFabricacions.Remove(paisFabricacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaisFabricacionExists(int id)
        {
            return _context.PaisFabricacions.Any(e => e.PaisFabricacionId == id);
        }
    }
}
