using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Proyecto_DSW.Models;
using Proyecto_DSW.Models.DTO;

namespace Proyecto_DSW.Controllers
{
    public class VentaController : Controller
    {
        private readonly IConfiguration _config;

        public VentaController(IConfiguration config)
        {
            _config = config;
        }

        private async Task<List<Venta>> ObtenerVentasPorUsuarioAsync(int idUsuario)
        {
            using var http = new HttpClient();
            http.BaseAddress = new Uri(_config["Services:URL_API"]);

            var mensaje = await http.GetAsync($"Venta/usuario/{idUsuario}");
            if (!mensaje.IsSuccessStatusCode)
                throw new Exception($"Error API: {mensaje.StatusCode}");

            var data = await mensaje.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Venta>>(data);
        }



        private async Task<VentaCompletaResponse> CrearVentaCompletaAsync(VentaCompletaRequest venta)
        {
            using var http = new HttpClient();
            http.BaseAddress = new Uri(_config["Services:URL_API"]);

            var contenido = new StringContent(
                JsonConvert.SerializeObject(venta),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var mensaje = await http.PostAsync("Venta/crear-completa", contenido);
            if (!mensaje.IsSuccessStatusCode)
                throw new Exception($"Error API: {mensaje.StatusCode}");

            var data = await mensaje.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<VentaCompletaResponse>(data,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        public IActionResult Index()
        {
            int? idUsuario = HttpContext.Session.GetInt32("usuario_id");
            if (idUsuario == null)
            {
                TempData["AccesoDenegado"] = "Debes iniciar sesión para ver tus ventas.";
                return RedirectToAction("Index", "Libro");
            }

            var ventas = ObtenerVentasPorUsuarioAsync(idUsuario.Value).Result;
            return View(ventas);
        }

        public IActionResult MisCompras()
        {
            int? idUsuario = HttpContext.Session.GetInt32("usuario_id");
            if (idUsuario == null)
            {
                TempData["AccesoDenegado"] = "Debes iniciar sesión para ver tus compras.";
                return RedirectToAction("Index", "Libro");
            }

            var ventas = ObtenerVentasPorUsuarioAsync(idUsuario.Value).Result;
            return View(ventas);
        }


        [HttpPost]
        public IActionResult Crear(int idUsuario, int idMetodoPago, string detalles)
        {
            var listaDetalles = JsonConvert.DeserializeObject<List<DetalleVentaRequest>>(detalles);

            var ventaRequest = new VentaCompletaRequest
            {
                IdUsuario = idUsuario,
                IdMetodoPago = idMetodoPago,
                FechaVenta = DateTime.Now,
                Detalles = listaDetalles
            };

            _ = CrearVentaCompletaAsync(ventaRequest).Result;


            return RedirectToAction("MisCompras");
        }

    }
}
