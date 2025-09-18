<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Identity;
=======
﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using Microsoft.AspNetCore.Identity;
>>>>>>> temporal
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

        //Constructor Principal Alquileres
        public AlquilersController(TiendaAlquilerDBContext context, UserManager<Usuario> userManager, ILogger<AlquilersController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }
<<<<<<< HEAD
        
        //Get Lista de Alquileres (Solo Mostrara el alquiler que se acaba de realizar para info de usuario (cliente))
        public async Task<IActionResult> Index(int? cocheId = null, string? usuarioId = null)
        {
            //Accedo a los datos de la Database para trabajar con ellos dentro del metodo
=======

        // GET: Alquilers
        public async Task<IActionResult> Index(int? cocheId = null, string usuarioId = null)
        {
>>>>>>> temporal
            var query = _context.Alquilers
                .Include(a => a.Coche)
                .Include(a => a.Usuario)
                .AsQueryable();

<<<<<<< HEAD
=======
            // Filtrar por coche y usuario si se proporcionan
>>>>>>> temporal
            if (cocheId.HasValue)
            {
                query = query.Where(a => a.CocheId == cocheId);
            }

            if (!string.IsNullOrEmpty(usuarioId))
            {
                query = query.Where(a => a.UsuarioId == usuarioId);
            }

<<<<<<< HEAD
=======
            // Ordenar por fecha más reciente y obtener el último alquiler
>>>>>>> temporal
            var ultimoAlquiler = await query
                .OrderByDescending(a => a.FechaAlquiler)
                .FirstOrDefaultAsync();

            if (ultimoAlquiler == null)
            {
<<<<<<< HEAD
                return View(new List<Alquiler>());
            }

            return View(new List<Alquiler> { ultimoAlquiler });
        }

        // GET Creacion alquiler
        public async Task<IActionResult> Create(int cocheId, string usuarioId)
        {
            var coche = await _context.Coches.FirstOrDefaultAsync(c => c.CocheId == cocheId);
=======
                return View(new List<Alquiler>()); // Lista vacía si no hay resultados
            }

            return View(new List<Alquiler> { ultimoAlquiler }); // Devolver una lista con el alquiler encontrado
        }



        // GET: Alquilers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alquiler = await _context.Alquilers
                .Include(a => a.Coche)
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(m => m.AlquilerId == id);
            if (alquiler == null)
            {
                return NotFound();
            }

            return View(alquiler);
        }

        // GET: Alquilers/Create
        [HttpGet]
        public async Task<IActionResult> Create(int cocheId, string usuarioId)
        {
            // Verificar si el coche existe en la base de datos
            var coche = await _context.Coches
                .FirstOrDefaultAsync(c => c.CocheId == cocheId); // Obtiene el coche por su ID

>>>>>>> temporal
            var usuario = await _userManager.FindByIdAsync(usuarioId);
            if (coche == null || usuario == null)
            {
            }
<<<<<<< HEAD
=======

>>>>>>> temporal
            var alquiler = new Alquiler
            {
                CocheId = cocheId,
                UsuarioId = usuarioId,
                Coche = coche,
                Usuario = usuario
            };
<<<<<<< HEAD
            ViewData["CocheMarca"] = alquiler.Coche?.Marca;
            ViewData["UsuarioNombre"] = alquiler.Usuario?.UserName;
            ViewData["Alquilers"] = await _context.Alquilers.Where(a => a.CocheId == cocheId).ToListAsync();
=======


            // Retornar la vista con la instancia de alquiler creada
>>>>>>> temporal
            return View(alquiler);
        }

<<<<<<< HEAD
        // POST Creacion Alquiler
=======


        // POST: Alquilers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

>>>>>>> temporal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlquilerId,CocheId,UsuarioId,FechaAlquiler,FechaDevolucion,NumeroTarjeta,FechaExpiracion,CVC")] Alquiler alquiler)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Buscar coche
                    var coche = await _context.Coches.FindAsync(alquiler.CocheId);
                    // Buscar usuario
                    var usuario = await _userManager.FindByIdAsync(alquiler.UsuarioId);
                    
                    // Validación de coche no encontrado
                    if (coche == null)
                    {
                        ModelState.AddModelError("", "El coche no existe");
                        await RecargaDatos(alquiler);
                        return View(alquiler);
                    }
                    
                    // Validacion de fechas
                    if (alquiler.FechaDevolucion <= alquiler.FechaAlquiler)
                    {
                        ModelState.AddModelError("", "La fecha de devolución debe ser posterior a la fecha de alquiler.");
                        await RecargaDatos(alquiler);
                        return View(alquiler);
                    }
                    
                    // Validacion de solapamiento de fechas
                    bool fechasSolapadas = await _context.Alquilers.AnyAsync(a =>
                        a.CocheId == alquiler.CocheId &&
                        (alquiler.FechaAlquiler < a.FechaDevolucion && alquiler.FechaDevolucion > a.FechaAlquiler));
                    if (fechasSolapadas)
                    {
                        ModelState.AddModelError("", "Este coche ya está alquilado en el rango de fechas seleccionado.");
                        await RecargaDatos(alquiler);
                        return View(alquiler);
                    }
                    
                    // Validacion de tarjeta
                    if (!ValidarTarjeta(alquiler))
                    {
                        ModelState.AddModelError("", "Los datos de la tarjeta no son válidos.");
                        await RecargaDatos(alquiler);
                        return View(alquiler);
                    }
                    
                    //SI TODO HA IDO BIEN:
                    // Calcular precio final
                    var diasAlquiler = (alquiler.FechaDevolucion.ToDateTime(TimeOnly.MinValue) - alquiler.FechaAlquiler.ToDateTime(TimeOnly.MinValue)).Days;
                    alquiler.PrecioFinal = coche.PrecioAlquiler * diasAlquiler;

                    // Guardar alquiler y redirigir a indice de alquileres
                    _context.Add(alquiler);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Logging de errores
                    ModelState.AddModelError("", $"Error inesperado: {ex.Message}");
                    await RecargaDatos(alquiler);
                    return View(alquiler);
                }
<<<<<<< HEAD
            }

            await RecargaDatos(alquiler);
            return View(alquiler);
        }
        
        //Metodo para recargar datos despues de validar si la validacion es incorrecta que se llama en el post
        private async Task RecargaDatos(Alquiler alquiler)
        {
            alquiler.Coche = await _context.Coches.FirstOrDefaultAsync(c => c.CocheId == alquiler.CocheId);
            alquiler.Usuario = await _userManager.FindByIdAsync(alquiler.UsuarioId);

            // Recargo la lista de alquileres relacionados
            ViewData["Alquilers"] = await _context.Alquilers.Where(a => a.CocheId == alquiler.CocheId).ToListAsync();
=======

                // Convertir las fechas de DateOnly a DateTime para poder realizar la operación de días
                DateTime fechaAlquiler = alquiler.FechaAlquiler.ToDateTime(new TimeOnly());
                DateTime fechaDevolucion = alquiler.FechaDevolucion.ToDateTime(new TimeOnly());

                // Validar que la fecha de devolución es posterior a la fecha de alquiler
                if (fechaDevolucion <= fechaAlquiler)
                {
                    ModelState.AddModelError("", "La fecha de devolución debe ser posterior a la fecha de alquiler.");
                    CargarListas(alquiler);
                    return View(alquiler);
                }

                // Verificar si ya existe un alquiler para el mismo coche en el rango de fechas
                var alquileresCoche = await _context.Alquilers
                    .Where(a => a.CocheId == alquiler.CocheId)
                    .ToListAsync();

                bool fechasSolapadas = alquileresCoche
                    .Any(a =>
                        (fechaAlquiler >= a.FechaAlquiler.ToDateTime(new TimeOnly()) && fechaAlquiler < a.FechaDevolucion.ToDateTime(new TimeOnly())) ||
                        (fechaDevolucion > a.FechaAlquiler.ToDateTime(new TimeOnly()) && fechaDevolucion <= a.FechaDevolucion.ToDateTime(new TimeOnly())) ||
                        (fechaAlquiler <= a.FechaAlquiler.ToDateTime(new TimeOnly()) && fechaDevolucion >= a.FechaDevolucion.ToDateTime(new TimeOnly()))
                    );

                if (fechasSolapadas)
                {
                    ModelState.AddModelError("", "Este coche ya está alquilado en el rango de fechas seleccionado.");
                    CargarListas(alquiler);
                    return View(alquiler);
                }

                // Validar los datos de la tarjeta (simulación)
                if (!ValidarTarjeta(alquiler))
                {
                    ModelState.AddModelError("", "Los datos de la tarjeta no son válidos.");
                    CargarListas(alquiler);
                    return View(alquiler);
                }
                if (!ModelState.IsValid)
                {
                    CargarListas(alquiler);
                    return View(alquiler);
                }

                // Calcular el número de días de alquiler
                var diasAlquiler = (fechaDevolucion - fechaAlquiler).Days;
                alquiler.PrecioFinal = coche.PrecioAlquiler * diasAlquiler;

                // Guardar el alquiler en la base de datos
                _context.Add(alquiler);
                await _context.SaveChangesAsync();

                // Redirigir a la lista de alquileres o donde se desee
                return RedirectToAction(nameof(Index));
            }

            // Si el modelo no es válido, recargar las listas y devolver la vista
            CargarListas(alquiler);
            // Redirigir a una acción para mostrar el último alquiler creado
            return View(alquiler);
        }












        public async Task<IActionResult> DetalleUltimoAlquiler(int cocheId)
        {
            // Obtener el último alquiler para el coche dado
            var ultimoAlquiler = await _context.Alquilers
                .Where(a => a.CocheId == cocheId)
                .OrderByDescending(a => a.AlquilerId)
                .FirstOrDefaultAsync();

            // Validar si existe un alquiler
            if (ultimoAlquiler == null)
            {
                return NotFound("No se encontró ningún alquiler para este coche.");
            }

            // Pasar el alquiler a la vista
            return View(new List<Alquiler> { ultimoAlquiler });
        }


        // Método para cargar las listas necesarias para la vista
        private void CargarListas(Alquiler alquiler)
        {
            // Obtener el coche y usuario previamente seleccionado
            ViewData["CocheId"] = new SelectList(_context.Coches, "CocheId", "Marca", alquiler.CocheId);
            ViewData["UsuarioId"] = new SelectList(_userManager.Users, "Id", "UserName", alquiler.UsuarioId);

            // Cargar las fechas de alquiler previas para el coche seleccionado
            ViewData["Alquilers"] = _context.Alquilers
                .Where(a => a.CocheId == alquiler.CocheId)
                .ToList();
>>>>>>> temporal
        }
        
        // Método para validar la tarjeta
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

        //Get Editar alquiler (No se usa de momento)
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

        //POST Editar alquiler (No se usa de momento)
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
