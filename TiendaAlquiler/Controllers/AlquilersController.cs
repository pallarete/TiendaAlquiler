using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TiendaAlquiler.Data;
using TiendaAlquiler.Models;


namespace TiendaAlquiler.Controllers
{
    public class AlquilersController(TiendaAlquilerDBContext context, UserManager<Usuario> userManager) : Controller
    {
        private readonly TiendaAlquilerDBContext _context = context;
        private readonly UserManager<Usuario> _userManager = userManager;
                    
        //Indice de Alquileres
        public async Task<IActionResult> Index(int? cocheId = null, string? usuarioId = null)
        {

            var query = _context.Alquilers
                .Include(a => a.Coche)
                .Include(a => a.Usuario)
                .AsQueryable();

            if (cocheId.HasValue)
            {
                query = query.Where(a => a.CocheId == cocheId);
            }

            if (!string.IsNullOrEmpty(usuarioId))
            {
                query = query.Where(a => a.UsuarioId == usuarioId);
            }

            var ultimoAlquiler = await query
                .OrderByDescending(a => a.FechaAlquiler)
                .FirstOrDefaultAsync();

            if (ultimoAlquiler == null)
            {
                return View(new List<Alquiler>());
            }

            return View(new List<Alquiler> { ultimoAlquiler });
        }

        // GET Create
        public async Task<IActionResult> Create(int cocheId, string usuarioId)
        {

            var coche = await _context.Coches.FirstOrDefaultAsync(c => c.CocheId == cocheId);
            var usuario = await _userManager.FindByIdAsync(usuarioId);

            if (coche == null || usuario == null)
            {
                return NotFound();
            }

            var alquiler = new Alquiler
            {
                CocheId = cocheId,
                UsuarioId = usuarioId,
                Coche = coche,
                Usuario = usuario
            };

            ViewData["CocheMarca"] = alquiler.Coche?.Marca;
            ViewData["UsuarioNombre"] = alquiler.Usuario?.UserName;

            ViewData["Alquilers"] = await _context.Alquilers.Where(a => a.CocheId == cocheId).ToListAsync();

            return View(alquiler);
        }

        // POST Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlquilerId,CocheId,UsuarioId,FechaAlquiler,FechaDevolucion,NumeroTarjeta,FechaExpiracion,CVC")] Alquiler alquiler)
        {
            if (alquiler == null)
            {
                return BadRequest("El alquiler no puede ser null");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var coche = await _context.Coches.FindAsync(alquiler.CocheId);
                    var usuario = await _userManager.FindByIdAsync(alquiler.UsuarioId);

                    if (coche == null)
                    {
                        ModelState.AddModelError("", "El coche no existe");
                        await RecargaDatos(alquiler);
                        return View(alquiler);
                    }

                    // Valido las fechas
                    if (alquiler.FechaDevolucion <= alquiler.FechaAlquiler)
                    {
                        ModelState.AddModelError("", "La fecha de devolución debe ser posterior a la fecha de alquiler.");
                        await RecargaDatos(alquiler);
                        return View(alquiler);
                    }

                    bool fechasSolapadas = await _context.Alquilers.AnyAsync(a =>
                        a.CocheId == alquiler.CocheId &&
                        (alquiler.FechaAlquiler < a.FechaDevolucion && alquiler.FechaDevolucion > a.FechaAlquiler));

                    if (fechasSolapadas)
                    {
                        ModelState.AddModelError("", "Este coche ya está alquilado en el rango de fechas seleccionado.");
                        await RecargaDatos(alquiler);
                        return View(alquiler);
                    }

                    if (!ValidarTarjeta(alquiler))
                    {
                        ModelState.AddModelError("", "Los datos de la tarjeta no son válidos.");
                        await RecargaDatos(alquiler);
                        return View(alquiler);
                    }

                    var diasAlquiler = (alquiler.FechaDevolucion.ToDateTime(TimeOnly.MinValue) - alquiler.FechaAlquiler.ToDateTime(TimeOnly.MinValue)).Days;


                    alquiler.PrecioFinal = coche.PrecioAlquiler * diasAlquiler;

                    _context.Add(alquiler);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al guardar el alquiler: {ex.Message}");
                    await RecargaDatos(alquiler);
                    return View(alquiler);
                }
            }
            await RecargaDatos(alquiler);
            return View(alquiler);
        }



        private async Task RecargaDatos(Alquiler alquiler)
        {
            alquiler.Coche = await _context.Coches.FirstOrDefaultAsync(c => c.CocheId == alquiler.CocheId);
            alquiler.Usuario = await _userManager.FindByIdAsync(alquiler.UsuarioId);

            ViewData["CocheMarca"] = alquiler.Coche?.Marca;
            ViewData["UsuarioNombre"] = alquiler.Usuario?.UserName;
            ViewData["Alquilers"] = await _context.Alquilers.Where(a => a.CocheId == alquiler.CocheId).ToListAsync();


        }
        // Método validacion tarjeta
        private static bool ValidarTarjeta(Alquiler alquiler)
        {
            if (alquiler.NumeroTarjeta.Length != 16)
            {
                return false;
            }

            var fechaExpiracion = alquiler.FechaExpiracion.Split('/');
            if (fechaExpiracion.Length != 2)
            {
                return false;
            }

            if (int.TryParse(fechaExpiracion[0], out int mesExpiracion) && int.TryParse(fechaExpiracion[1], out int anioExpiracion))
            {
                if (mesExpiracion < 1 || mesExpiracion > 12)
                {
                    return false;
                }

                if (anioExpiracion < DateTime.Now.Year % 100)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlquilerId,CocheId,UsuarioId,FechaAlquiler,FechaDevolucion,NumeroTarjeta,FechaExpiracion,CVC,PrecioFinal")] Alquiler alquiler)
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
            return View(alquiler);
        }

        private bool AlquilerExists(int id)
        {
            return _context.Alquilers.Any(e => e.AlquilerId == id);
        }
    }
}
