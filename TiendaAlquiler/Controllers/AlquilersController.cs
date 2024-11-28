﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
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


        public AlquilersController(TiendaAlquilerDBContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Alquilers
        public async Task<IActionResult> Index(int? cocheId = null, string usuarioId = null)
        {
            var query = _context.Alquilers
                .Include(a => a.Coche)
                .Include(a => a.Usuario)
                .AsQueryable();

            // Filtrar por coche y usuario si se proporcionan
            if (cocheId.HasValue)
            {
                query = query.Where(a => a.CocheId == cocheId);
            }

            if (!string.IsNullOrEmpty(usuarioId))
            {
                query = query.Where(a => a.UsuarioId == usuarioId);
            }

            // Ordenar por fecha más reciente y obtener el último alquiler
            var ultimoAlquiler = await query
                .OrderByDescending(a => a.FechaAlquiler)
                .FirstOrDefaultAsync();

            if (ultimoAlquiler == null)
            {
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

            // Verificar si el coche y el usuario existen
            var coche = await _context.Coches.FindAsync(cocheId);
            var usuario = await _userManager.FindByIdAsync(usuarioId);

            if (coche == null || usuario == null)
            {
                return NotFound();
            }

            // Crear una instancia de Alquiler prellenada con el coche y el usuario seleccionados
            var alquiler = new Alquiler
            {
                CocheId = cocheId,
                UsuarioId = usuarioId,
            };

            ViewData["CocheId"] = new SelectList(_context.Coches, "CocheId", "Marca", alquiler.CocheId);
            ViewData["UsuarioId"] = new SelectList(await _userManager.Users.ToListAsync(), "Id", "UserName", alquiler.UsuarioId);
            // Cargar los alquileres previos del coche si ya existe uno
            ViewData["Alquilers"] = _context.Alquilers
                .Where(a => a.CocheId == cocheId)  // Reemplazar con el CocheId del coche que se pasa al crear el alquiler
                .ToList();

            return View(alquiler);

        }

        // POST: Alquilers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlquilerId,CocheId,UsuarioId,FechaAlquiler,FechaDevolucion,NumeroTarjeta,FechaExpiracion,CVC")] Alquiler alquiler)
        {
            if (ModelState.IsValid)
            {
                // Obtener el coche desde la base de datos utilizando el CocheId
                var coche = await _context.Coches.FindAsync(alquiler.CocheId);

                // Si el coche no fue encontrado
                if (coche == null)
                {
                    ModelState.AddModelError("", "El coche no existe");
                    CargarListas(alquiler); // Método para recargar los datos necesarios en caso de error
                    return View(alquiler);
                }

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
        }



        // Método para simular la validación de la tarjeta
        private bool ValidarTarjeta(Alquiler alquiler)
        {
            // Validación básica: por ejemplo, verificamos si el número de tarjeta tiene 16 dígitos.
            if (alquiler.NumeroTarjeta.Length != 16)
            {
                return false;
            }

            // Validar la fecha de expiración
            // El formato debe ser MM/AA, lo dividimos para obtener el mes y el año
            var fechaExpiracion = alquiler.FechaExpiracion.Split('/');
            if (fechaExpiracion.Length != 2)
            {
                return false;
            }

            if (int.TryParse(fechaExpiracion[0], out int mesExpiracion) && int.TryParse(fechaExpiracion[1], out int anioExpiracion))
            {
                // Validar que el mes está entre 1 y 12
                if (mesExpiracion < 1 || mesExpiracion > 12)
                {
                    return false;
                }

                // Validar que el año de expiración sea mayor o igual al año actual
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
            ViewData["CocheId"] = new SelectList(_context.Coches, "CocheId", "Marca", alquiler.CocheId);
            ViewData["UsuarioId"] = new SelectList(await _userManager.Users.ToListAsync(), "Id", "Rol");


            return View(alquiler);
        }

        // POST: Alquilers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlquilerId,CocheId,UsuarioId,FechaAlquiler,FechaDevolucion,PrecioFinal")] Alquiler alquiler)
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
            ViewData["CocheId"] = new SelectList(_context.Coches, "CocheId", "Marca", alquiler.CocheId);
            ViewData["UsuarioId"] = new SelectList(await _userManager.Users.ToListAsync(), "Id", "Rol");

            return View(alquiler);
        }

        // GET: Alquilers/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Alquilers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alquiler = await _context.Alquilers.FindAsync(id);
            if (alquiler != null)
            {
                _context.Alquilers.Remove(alquiler);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlquilerExists(int id)
        {
            return _context.Alquilers.Any(e => e.AlquilerId == id);
        }
    }
}
