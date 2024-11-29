using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TiendaAlquiler.Data;
using TiendaAlquiler.Models;
using Microsoft.Extensions.Logging; // Agrega esta referencia

namespace TiendaAlquiler.Controllers
{
    public class AlquilersController : Controller
    {
        private readonly TiendaAlquilerDBContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<AlquilersController> _logger; // Inyectamos ILogger

        // Constructor del controlador con ILogger
        public AlquilersController(TiendaAlquilerDBContext context, UserManager<Usuario> userManager, ILogger<AlquilersController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger; // Asignamos el logger
        }

        // GET: Alquilers
        public async Task<IActionResult> Index(int? cocheId = null, string usuarioId = null)
        {
            _logger.LogInformation("Ejecutando el método Index de AlquilersController."); // Registro informativo

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
                _logger.LogWarning("No se encontraron alquileres en el método Index para los filtros dados."); // Registro de advertencia
                return View(new List<Alquiler>());
            }

            return View(new List<Alquiler> { ultimoAlquiler });
        }

        // GET: Alquilers/Create
        public async Task<IActionResult> Create(int cocheId, string usuarioId)
        {
            _logger.LogInformation($"Ejecutando el método Create para CocheId: {cocheId} y UsuarioId: {usuarioId}");

            var coche = await _context.Coches.FirstOrDefaultAsync(c => c.CocheId == cocheId);
            var usuario = await _userManager.FindByIdAsync(usuarioId);

            if (coche == null || usuario == null)
            {
                _logger.LogError("No se encontró el coche o el usuario con los Id proporcionados."); // Registro de error
                return NotFound();
            }

            var alquiler = new Alquiler
            {
                CocheId = cocheId,
                UsuarioId = usuarioId
            };

            ViewData["CocheMarca"] = coche.Marca;
            ViewData["UsuarioNombre"] = usuario.UserName;

            ViewData["Alquilers"] = await _context.Alquilers
                .Where(a => a.CocheId == cocheId)
                .ToListAsync();

            return View(alquiler);
        }

        // POST: Alquilers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlquilerId,CocheId,UsuarioId,FechaAlquiler,FechaDevolucion,NumeroTarjeta,FechaExpiracion,CVC")] Alquiler alquiler)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation($"Creando un nuevo alquiler con CocheId: {alquiler.CocheId} y UsuarioId: {alquiler.UsuarioId}");

                    var coche = await _context.Coches.FindAsync(alquiler.CocheId);

                    if (coche == null)
                    {
                        _logger.LogError($"No se encontró el coche con CocheId: {alquiler.CocheId}");
                        ModelState.AddModelError("", "El coche no existe");
                        return View(alquiler);
                    }

                    // Validación de las fechas
                    DateTime fechaAlquiler = alquiler.FechaAlquiler.ToDateTime(new TimeOnly());
                    DateTime fechaDevolucion = alquiler.FechaDevolucion.ToDateTime(new TimeOnly());

                    if (fechaDevolucion <= fechaAlquiler)
                    {
                        _logger.LogWarning("La fecha de devolución es anterior o igual a la fecha de alquiler.");
                        ModelState.AddModelError("", "La fecha de devolución debe ser posterior a la fecha de alquiler.");
                        return View(alquiler);
                    }

                    bool fechasSolapadas = await _context.Alquilers.AnyAsync(a =>
                        a.CocheId == alquiler.CocheId &&
                        (fechaAlquiler < a.FechaDevolucion.ToDateTime(new TimeOnly()) && fechaDevolucion > a.FechaAlquiler.ToDateTime(new TimeOnly()))
                    );

                    if (fechasSolapadas)
                    {
                        _logger.LogWarning("Las fechas de alquiler se solapan con otro alquiler existente.");
                        ModelState.AddModelError("", "Este coche ya está alquilado en el rango de fechas seleccionado.");
                        return View(alquiler);
                    }

                    if (!ValidarTarjeta(alquiler))
                    {
                        _logger.LogWarning("Los datos de la tarjeta son inválidos.");
                        ModelState.AddModelError("", "Los datos de la tarjeta no son válidos.");
                        return View(alquiler);
                    }

                    var diasAlquiler = (fechaDevolucion - fechaAlquiler).Days;
                    alquiler.PrecioFinal = coche.PrecioAlquiler * diasAlquiler;

                    _context.Add(alquiler);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Alquiler creado exitosamente para CocheId: {alquiler.CocheId}, UsuarioId: {alquiler.UsuarioId}");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error al crear el alquiler: {ex.Message}"); // Registro de error
                    ModelState.AddModelError("", $"Error al guardar el alquiler: {ex.Message}");
                    return View(alquiler);
                }
            }

            return View(alquiler);
        }

        // Método para validar la tarjeta
        private bool ValidarTarjeta(Alquiler alquiler)
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

            _logger.LogInformation($"Editando alquiler con ID: {id}");

            ViewData["CocheId"] = new SelectList(_context.Coches, "CocheId", "Marca", alquiler.CocheId);
            ViewData["UsuarioId"] = new SelectList(await _userManager.Users.ToListAsync(), "Id", "Rol");

            return View(alquiler);
        }

        // POST: Alquilers/Edit/5
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
                    _logger.LogInformation($"Editando el alquiler con ID: {alquiler.AlquilerId}");

                    _context.Update(alquiler);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Alquiler con ID: {alquiler.AlquilerId} editado exitosamente.");
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
