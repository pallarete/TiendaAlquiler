﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;
using TiendaAlquiler.Data;
using TiendaAlquiler.Models;


namespace TiendaAlquiler.Controllers
{
    public class CochesController : Controller
    {
        private readonly TiendaAlquilerDBContext _context;
        public CochesController(TiendaAlquilerDBContext context)
        {
            _context = context;
        }

        //GET Lista de Coches
        public async Task<IActionResult> Index(int? paisId, int? decadaId, int? colorId, int? carroceriaId)
        {
            // Ordeno todos y cada uno de los filtros alfabéticamente
            ViewBag.Paises = _context.Paises
                .OrderBy(p => p.Nombre)
                .Select(p => new SelectListItem { Value = p.PaisId.ToString(), Text = p.Nombre })
                .ToList();

            ViewBag.Decadas = _context.Decada
                .OrderBy(d => d.AnioInicio)
                .Select(d => new SelectListItem { Value = d.DecadaId.ToString(), Text = d.AnioInicio.ToString() })
                .ToList();

            ViewBag.Colores = _context.Colors
                .OrderBy(c => c.Nombre)
                .Select(c => new SelectListItem { Value = c.ColorId.ToString(), Text = c.Nombre })
                .ToList();

            ViewBag.Carrocerias = _context.Carroceria
                .OrderBy(car => car.Tipo)
                .Select(car => new SelectListItem { Value = car.CarroceriaId.ToString(), Text = car.Tipo })
                .ToList();

            var coches = _context.Coches
                .Include(c => c.Pais)
                .Include(c => c.Decada)
                .Include(c => c.Color)
                .Include(c => c.Carroceria)
                .AsQueryable(); //Convierto a una consulta que se puede modificar

            if (paisId.HasValue)
            {
                coches = coches.Where(c => c.PaisId == paisId.Value);
            }

            if (decadaId.HasValue)
            {
                coches = coches.Where(c => c.DecadaId == decadaId.Value);
            }
            if (colorId.HasValue)
            {
                coches = coches.Where(c => c.ColorId == colorId.Value);
            }
            if (carroceriaId.HasValue)
            {
                coches = coches.Where(c => c.CarroceriaId == carroceriaId.Value);
            }

            // Ordenar por marca alfabéticamente dentro de la tabla
            coches = coches.OrderBy(c => c.Marca);
            return View(await coches.ToListAsync());
        }

        // GET: Obtengo la vista de los detalles de cada coche
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
                .Include(c => c.Pais)
                .Include(c => c.Fotos)
                .Include(c => c.Alquilers)
                .FirstOrDefaultAsync(m => m.CocheId == id);
            if (coche == null)
            {
                return NotFound();
            }
            return View(coche);
        }

        // GET: Crear un coche solo si eres "Admin"
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Ordeno por el campo correspondiente para cada lista desplegable
            ViewData["CarroceriaId"] = new SelectList(_context.Carroceria.OrderBy(c => c.Tipo), "CarroceriaId", "Tipo");
            ViewData["ColorId"] = new SelectList(_context.Colors.OrderBy(c => c.Nombre), "ColorId", "Nombre");
            ViewData["DecadaId"] = new SelectList(_context.Decada.OrderBy(c => c.AnioInicio), "DecadaId", "AnioInicio");
            ViewData["PaisId"] = new SelectList(_context.Paises.OrderBy(c => c.Nombre), "PaisId", "Nombre");
            return View();
        }

        // POST: Crear un coche solo si eres "Admin"
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("CocheId,Marca,Modelo,AnioFabricacion,PrecioAlquiler,EstaAlquilado,ColorId,CarroceriaId,DecadaId,PaisId")] Coche coche, IFormFile[] archivos)
        {
            if (ModelState.IsValid)
            {
                //Guardamos el coche en la base de datos
                _context.Add(coche);
                await _context.SaveChangesAsync();
                //Procesamos las fotos solo si se han cargado archivos
                if (archivos != null && archivos.Length != 0)
                {
                    foreach (var archivo in archivos)
                    {
                        if (archivo.Length > 0)
                        {
                            //ruta para guardar el archivo
                            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(archivo.FileName);
                            var filePath = Path.Combine("wwwroot/imagenes", uniqueFileName);

                            //Creamos una imagen redimensionada
                            using (var image = Image.FromStream(archivo.OpenReadStream()))
                            {
                                //Establecemos el tamaño
                                int width = 1200;
                                int height = 800;
                                using var resizedImage = new Bitmap(image, new Size(width, height));
                                //guarda la imagen redimensionada
                                resizedImage.Save(filePath, ImageFormat.Jpeg);
                            }

                            //Creamos una nueva foto y la asociamos al coche

                            Foto foto = new()
                            {
                                CocheId = coche.CocheId, //Asigno el CocheId a la foto
                                RutaAcceso = Path.Combine("imagenes", uniqueFileName)
                            };
                            //Agrego la foto al contexto
                            _context.Fotos.Add(foto);
                        }
                    }
                    //Guardo las fotos en la base de datos
                    await _context.SaveChangesAsync();
                }
                //Redirigo al listado de cohes
                return RedirectToAction(nameof(Index));
            }
            //En caso de error muestro formulario de creacion
            ViewBag.CarroceriaId = new SelectList(_context.Carroceria.OrderBy(c => c.Tipo), "CarroceriaId", "Tipo", coche.CarroceriaId);
            ViewBag.ColorId = new SelectList(_context.Colors.OrderBy(c => c.Nombre), "ColorId", "Nombre", coche.ColorId);
            ViewBag.DecadaId = new SelectList(_context.Decada.OrderBy(c => c.AnioInicio), "DecadaId", "AnioInicio", coche.DecadaId);
            ViewBag.PaisId = new SelectList(_context.Paises.OrderBy(c => c.Nombre), "PaisId", "Nombre", coche.PaisId);
            return View(coche);
        }

        // GET: Edito el coche una vez creado
        [Authorize(Roles = "Admin")]
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
            ViewBag.CarroceriaId = new SelectList(_context.Carroceria.OrderBy(c => c.Tipo), "CarroceriaId", "Tipo", coche.CarroceriaId);
            ViewBag.ColorId = new SelectList(_context.Colors.OrderBy(c => c.Nombre), "ColorId", "Nombre", coche.ColorId);
            ViewBag.DecadaId = new SelectList(_context.Decada.OrderBy(c => c.AnioInicio), "DecadaId", "AnioInicio", coche.DecadaId);
            ViewBag.PaisId = new SelectList(_context.Paises.OrderBy(c => c.Nombre), "PaisId", "Nombre", coche.PaisId);
            return View(coche);
        }

        // POST: Edito el coche una vez creado
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("CocheId,Marca,Modelo,AnioFabricacion,PrecioAlquiler,EstaAlquilado,ColorId,CarroceriaId,DecadaId,PaisId,Description")] Coche coche, IFormFile[] archivos)
        {
            if (id != coche.CocheId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizo los datos del coche en la base de datos
                    _context.Update(coche);
                    await _context.SaveChangesAsync();

                    // Eliminar fotos anteriores si es necesario
                    var fotosExistentes = _context.Fotos.Where(f => f.CocheId == coche.CocheId).ToList();
                    _context.Fotos.RemoveRange(fotosExistentes);
                    await _context.SaveChangesAsync();

                    // Proceso las fotos solo si se han cargado archivos
                    if (archivos != null && archivos.Length != 0)
                    {
                        foreach (var archivo in archivos)
                        {
                            if (archivo.Length > 0)
                            {
                                // Genero un nombre único para el archivo
                                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(archivo.FileName);
                                var filePath = Path.Combine("wwwroot/imagenes", uniqueFileName);

                                // Creo una imagen redimensionada
                                using (var image = Image.FromStream(archivo.OpenReadStream()))
                                {
                                    int width = 1200;
                                    int height = 800;
                                    using var resizedImage = new Bitmap(image, new Size(width, height));
                                    // Guardola imagen redimensionada
                                    resizedImage.Save(filePath, ImageFormat.Jpeg);
                                }

                                // Creo una nueva foto y la asocio al coche
                                Foto foto = new()
                                {
                                    CocheId = coche.CocheId,
                                    RutaAcceso = Path.Combine("imagenes", uniqueFileName)
                                };

                                // Agrego la foto al contexto
                                _context.Fotos.Add(foto);
                            }
                        }

                        // Guardo las fotos en la base de datos
                        await _context.SaveChangesAsync();
                    }
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
            // Si hay errores, vuelvo a cargar los datos relacionados
            ViewBag.CarroceriaId = new SelectList(_context.Carroceria.OrderBy(c => c.Tipo), "CarroceriaId", "Tipo", coche.CarroceriaId);
            ViewBag.ColorId = new SelectList(_context.Colors.OrderBy(c => c.Nombre), "ColorId", "Nombre", coche.ColorId);
            ViewBag.DecadaId = new SelectList(_context.Decada.OrderBy(c => c.AnioInicio), "DecadaId", "AnioInicio", coche.DecadaId);
            ViewBag.PaisId = new SelectList(_context.Paises.OrderBy(c => c.Nombre), "PaisId", "Nombre", coche.PaisId);
            return View(coche);
        }

        // GET: Borrar Coche
        [Authorize(Roles = "Admin")]
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
                .Include(c => c.Pais)
                .FirstOrDefaultAsync(m => m.CocheId == id);
            if (coche == null)
            {
                return NotFound();
            }

            return View(coche);
        }

        // POST: Borrar Coche Vista
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var cocheAlquilado = await _context.Alquilers.AnyAsync(c => c.CocheId == id);
                if (cocheAlquilado)
                {
                    //Envio un mensaje a TemdData
                    TempData["ErrorMessage"] = "No se puede eliminar este coche porque tiene alquileres asociados";

                    return RedirectToAction("Delete", new { id });
                }
                var coche = await _context.Coches.FindAsync(id);
                if (coche != null)
                {
                    _context.Coches.Remove(coche);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));

            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));

            }
        }

        private bool CocheExists(int id)
        {
            return _context.Coches.Any(e => e.CocheId == id);
        }

        //Puedo cambiar La descripcion Del coche
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActualizarDescripcion(int CocheId, string Descripcion)
        {
            var coche = await _context.Coches.FindAsync(CocheId);
            if (coche == null)
            {
                return NotFound();
            }

            coche.Description = Descripcion;
            _context.Update(coche);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = coche.CocheId });
        }

    }
}
