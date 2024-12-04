using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TiendaAlquiler.Data;
using TiendaAlquiler.Models;


namespace TiendaAlquiler.Controllers
{
    public class AlquilersController : Controller
    {
        private readonly TiendaAlquilerDBContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<AlquilersController> _logger;
        public AlquilersController(TiendaAlquilerDBContext context, UserManager<Usuario> userManager, ILogger<AlquilersController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }
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
        // GET Vista creacion alquiler
        public async Task<IActionResult> Create(int cocheId, string usuarioId)
        {
            _logger.LogInformation("Entrando al método Create");
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

        // POST Vista creacion Alquiler
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlquilerId,CocheId,UsuarioId,FechaAlquiler,FechaDevolucion,NumeroTarjeta,FechaExpiracion,CVC")] Alquiler alquiler)
        {
            _logger.LogInformation("Entrando al método Create");
            // Logging al inicio del método
            Console.WriteLine("Entrando al método Create");
            Console.WriteLine($"Datos recibidos: CocheId={alquiler.CocheId}, UsuarioId={alquiler.UsuarioId}, FechaAlquiler={alquiler.FechaAlquiler}, FechaDevolucion={alquiler.FechaDevolucion}");

            if (ModelState.IsValid)
            {
                try
                {
                    // Buscar coche
                    var coche = await _context.Coches.FindAsync(alquiler.CocheId);
                    Console.WriteLine(coche == null ? "Coche no encontrado" : $"Coche encontrado: {coche.Marca}, PrecioAlquiler: {coche.PrecioAlquiler}");

                    // Buscar usuario
                    var usuario = await _userManager.FindByIdAsync(alquiler.UsuarioId);
                    Console.WriteLine(usuario == null ? "Usuario no encontrado" : $"Usuario encontrado: {usuario.UserName}");

                    // Validación: coche no encontrado
                    if (coche == null)
                    {
                        ModelState.AddModelError("", "El coche no existe");
                        await RecargaDatos(alquiler);
                        return View(alquiler);
                    }

                    // Validar fechas
                    if (alquiler.FechaDevolucion <= alquiler.FechaAlquiler)
                    {
                        Console.WriteLine("Error: Fecha de devolución anterior o igual a la fecha de alquiler.");
                        ModelState.AddModelError("", "La fecha de devolución debe ser posterior a la fecha de alquiler.");
                        await RecargaDatos(alquiler);
                        return View(alquiler);
                    }

                    // Verificar solapamiento de fechas
                    bool fechasSolapadas = await _context.Alquilers.AnyAsync(a =>
                        a.CocheId == alquiler.CocheId &&
                        (alquiler.FechaAlquiler < a.FechaDevolucion && alquiler.FechaDevolucion > a.FechaAlquiler));

                    if (fechasSolapadas)
                    {
                        Console.WriteLine("Error: Fechas de alquiler solapadas.");
                        ModelState.AddModelError("", "Este coche ya está alquilado en el rango de fechas seleccionado.");
                        await RecargaDatos(alquiler);
                        return View(alquiler);
                    }

                    // Validar tarjeta
                    if (!ValidarTarjeta(alquiler))
                    {
                        Console.WriteLine("Error: Datos de la tarjeta inválidos.");
                        ModelState.AddModelError("", "Los datos de la tarjeta no son válidos.");
                        await RecargaDatos(alquiler);
                        return View(alquiler);
                    }

                    // Calcular precio final
                    var diasAlquiler = (alquiler.FechaDevolucion.ToDateTime(TimeOnly.MinValue) - alquiler.FechaAlquiler.ToDateTime(TimeOnly.MinValue)).Days;
                    alquiler.PrecioFinal = coche.PrecioAlquiler * diasAlquiler;
                    Console.WriteLine($"Precio final calculado: {alquiler.PrecioFinal}");

                    // Guardar alquiler
                    _context.Add(alquiler);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Alquiler guardado correctamente en la base de datos.");

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Logging de errores
                    Console.WriteLine($"Excepción capturada: {ex.Message}");
                    Console.WriteLine($"StackTrace: {ex.StackTrace}");

                    ModelState.AddModelError("", $"Error inesperado: {ex.Message}");
                    await RecargaDatos(alquiler);
                    return View(alquiler);
                }
            }
            

            // Si el modelo no es válido, loggear los errores
            Console.WriteLine("ModelState inválido. Errores:");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"- {error.ErrorMessage}");
            }

            await RecargaDatos(alquiler);
            return View(alquiler);
        }

        private async Task RecargaDatos(Alquiler alquiler)
        {
            alquiler.Coche = await _context.Coches.FirstOrDefaultAsync(c => c.CocheId == alquiler.CocheId);
            alquiler.Usuario = await _userManager.FindByIdAsync(alquiler.UsuarioId);

           

            // Asigna la lista de alquileres relacionados, si es necesario
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
