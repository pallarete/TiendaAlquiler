using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TiendaAlquiler.Models;

namespace TiendaAlquiler.Controllers
{
    public class PagoController : Controller
    {
        // Vista para mostrar el formulario de pago
        public IActionResult CrearPago()
        {
            var modeloPago = new Pago
            {
                Monto = 50.0m // Ejemplo del monto a pagar
            };
            return View(modeloPago);
        }

        // Acción POST para procesar el pago
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcesarPago([Bind("NumeroTarjeta,NombreTitular,FechaVencimiento,CodigoSeguridad,Monto")] Pago pago)
        {
            if (ModelState.IsValid)
            {
                // Simulación de la verificación del pago
                if (pago.NumeroTarjeta.StartsWith("4"))
                {
                    // Simulamos un pago exitoso
                    TempData["Pago"] = JsonConvert.SerializeObject(pago); // Guardamos el objeto Pago en TempData
                    TempData["Mensaje"] = "¡Pago realizado con éxito!"; // Guardamos el mensaje

                    // Agregar un mensaje de depuración para ver si está redirigiendo
                    Console.WriteLine("Pago realizado con éxito. Redirigiendo a ResultadoPago...");

                    return RedirectToAction("ResultadoPago"); // Redirigimos a la acción de ResultadoPago
                }
                else
                {
                    // Simulamos un error en el pago
                    TempData["Pago"] = JsonConvert.SerializeObject(pago);
                    TempData["Mensaje"] = "Pago fallido. Verifique los datos de la tarjeta."; // Guardamos el mensaje de error

                    // Agregar un mensaje de depuración para ver si está redirigiendo
                    Console.WriteLine("Pago fallido. Redirigiendo a ResultadoPago...");

                    return RedirectToAction("ResultadoPago"); // Redirigimos a la acción de ResultadoPago
                }
            }

            // Si el modelo no es válido, volvemos al formulario
            return View("CrearPago", pago);
        }

        // Vista para mostrar el resultado del pago
        public IActionResult ResultadoPago()
        {
            // Recuperamos los datos del pago y el mensaje de TempData
            var pago = JsonConvert.DeserializeObject<Pago>(TempData["Pago"] as string);
            var mensaje = TempData["Mensaje"] as string;

            // Agregar un mensaje de depuración para asegurarnos de que los datos están presentes
            Console.WriteLine($"ResultadoPago: Pago = {pago?.NumeroTarjeta}, Mensaje = {mensaje}");

            ViewBag.Mensaje = mensaje; // Recuperamos el mensaje de TempData

            return View(pago); // Pasamos el modelo de pago a la vista
        }
    }
}
