using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TiendaAlquiler.Models;
using System.Drawing; // Asegúrate de incluir este espacio de nombres
using System.Drawing.Imaging;
using TiendaAlquiler.Data;
using Microsoft.AspNetCore.Authorization;


namespace TiendaAlquiler.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FotoesController : Controller
    {
        private readonly TiendaAlquilerDBContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FotoesController(TiendaAlquilerDBContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Fotoes
        public async Task<IActionResult> Index()
        {
            var tiendaAlquilerDBContext = _context.Fotos.Include(f => f.Coche);
            return View(await tiendaAlquilerDBContext.ToListAsync());
        }

        // GET: Fotoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foto = await _context.Fotos
                .Include(f => f.Coche)
                .FirstOrDefaultAsync(m => m.FotoId == id);
            if (foto == null)
            {
                return NotFound();
            }

            return View(foto);
        }

        // GET: Fotoes/Create
        public IActionResult Create()
        {
            ViewData["CocheId"] = new SelectList(_context.Coches, "CocheId", "Marca");
            return View();
        }

        // POST: Fotoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CocheId")] Foto foto, IFormFile archivo)
        {

            if (archivo != null && archivo.Length > 0)
            {
                //Ruta para guardar el archivo
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(archivo.FileName);
                var filePath = Path.Combine("wwwroot/imagenes", uniqueFileName);

                //Creamos una imagen redimensionada

                using (var image = Image.FromStream(archivo.OpenReadStream()))
                {
                    //Le digo que tamaño quiero
                    int width = 300;
                    int height = 300;
                    using (var resizedImage = new Bitmap(image, new Size(width, height)))
                    {
                        //guarda la imagen redimensionada
                        resizedImage.Save(filePath, ImageFormat.Jpeg);
                    }
                }
                foto.RutaAcceso = Path.Combine("imagenes", uniqueFileName);

                ////Guardamos el archivo en el servidor
                //using (var fileStream = new FileStream(filePath, FileMode.Create))
                //{
                //    await archivo.CopyToAsync(fileStream);
                //}
                ////Asignar la ruta de acceso a la entidad foto
            }
            if (ModelState.IsValid)
            {
                _context.Add(foto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ////En caso de error , volver a cargar coches
            //ViewData["CocheId"] = new SelectList(_context.Coches, "CocheId", "Marca", foto.CocheId);
            return View(foto);
        }

        // GET: Fotoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foto = await _context.Fotos.FindAsync(id);
            if (foto == null)
            {
                return NotFound();
            }
            ViewData["CocheId"] = new SelectList(_context.Coches, "CocheId", "Marca", foto.CocheId);
            return View(foto);
        }

        // POST: Fotoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FotoId,CocheId,RutaAcceso")] Foto foto)
        {
            if (id != foto.FotoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(foto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FotoExists(foto.FotoId))
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
            ViewData["CocheId"] = new SelectList(_context.Coches, "CocheId", "Marca", foto.CocheId);
            return View(foto);
        }

        // GET: Fotoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foto = await _context.Fotos
                .Include(f => f.Coche)
                .FirstOrDefaultAsync(m => m.FotoId == id);
            if (foto == null)
            {
                return NotFound();
            }

            return View(foto);
        }

        // POST: Fotoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var foto = await _context.Fotos.FindAsync(id);
            if (foto != null)
            {
                _context.Fotos.Remove(foto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FotoExists(int id)
        {
            return _context.Fotos.Any(e => e.FotoId == id);
        }
    }
}
