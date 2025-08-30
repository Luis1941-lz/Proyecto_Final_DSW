using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Proyecto_DSW.Models;
using Proyecto_DSW.Models.DTO;

namespace Proyecto_DSW.Controllers
{
    public class CarritoController : Controller
    {
        private readonly IConfiguration _config;

        public CarritoController(IConfiguration config)
        {
            _config = config;
        }

        private async Task<List<Carrito>> ObtenerCarritoPorUsuarioAsync(int idUsuario)
        {
            using var http = new HttpClient();
            http.BaseAddress = new Uri(_config["Services:URL_API"]);
            var mensaje = await http.GetAsync($"Carrito/Usuario/{idUsuario}");

            if (mensaje.StatusCode == System.Net.HttpStatusCode.NotFound)
                return new List<Carrito>(); 

            if (!mensaje.IsSuccessStatusCode)
                throw new Exception($"Error API: {mensaje.StatusCode}");

            var data = await mensaje.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Carrito>>(data);
        }

        private async Task<List<MetodoPago>> ObtenerMetodosDePago()
        {
            using var http = new HttpClient();
            http.BaseAddress = new Uri(_config["Services:URL_API"]);

            var mensaje = await http.GetAsync("Metodopago");
            if (!mensaje.IsSuccessStatusCode)
                throw new Exception($"Error al obtener métodos de pago: {mensaje.StatusCode}");

            var data = await mensaje.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<MetodoPago>>(data);
        }


        private async Task<Carrito> InsertarCarritoAsync(Carrito carrito)
        {
            using var http = new HttpClient();
            http.BaseAddress = new Uri(_config["Services:URL_API"]);

            var contenido = new StringContent(
                JsonConvert.SerializeObject(carrito),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var mensaje = await http.PostAsync("Carrito/agregar", contenido);

            if (!mensaje.IsSuccessStatusCode)
                throw new Exception($"Error API: {mensaje.StatusCode}");

            var data = await mensaje.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Carrito>(data);
        }

        private async Task<bool> VaciarCarritoAsync(int idUsuario)
        {
            using var http = new HttpClient();
            http.BaseAddress = new Uri(_config["Services:URL_API"]);
            var mensaje = await http.DeleteAsync($"Carrito/Vaciar/{idUsuario}");

            return mensaje.IsSuccessStatusCode;
        }


        public IActionResult Index()
        {
            int? idUsuario = HttpContext.Session.GetInt32("usuario_id");
            if (idUsuario == null)
            {
                TempData["AccesoDenegado"] = "Debes iniciar sesión para ver tu carrito.";
                return RedirectToAction("Index", "Libro");
            }

            var listado = ObtenerCarritoPorUsuarioAsync(idUsuario.Value).Result;

            int totalItems = listado.Sum(x => x.Cantidad);
            HttpContext.Session.SetInt32("carrito_cantidad", totalItems);

            if (listado.Count == 0)
                ViewBag.Mensaje = "Tu carrito está vacío.";

            return View(listado);
        }

        [HttpPost]
        public IActionResult Agregar(int idLibro, int cantidad = 1)
        {
            int? idUsuario = HttpContext.Session.GetInt32("usuario_id");
            if (idUsuario == null)
                return RedirectToAction("Index", "Libro");

            var carrito = new Carrito
            {
                IdCarrito = 0,
                IdUsuario = idUsuario.Value,
                IdLibro = idLibro,
                Cantidad = cantidad
            };

            _ = InsertarCarritoAsync(carrito).Result;

            var listado = ObtenerCarritoPorUsuarioAsync(idUsuario.Value).Result;
            int totalItems = listado.Sum(x => x.Cantidad);

            HttpContext.Session.SetInt32("carrito_cantidad", totalItems);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Vaciar()
        {
            int? idUsuario = HttpContext.Session.GetInt32("usuario_id");
            if (idUsuario != null)
            {
                _ = VaciarCarritoAsync(idUsuario.Value).Result;

                HttpContext.Session.SetInt32("carrito_cantidad", 0);
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmarCompra()
        {
            int? idUsuario = HttpContext.Session.GetInt32("usuario_id");
            if (idUsuario == null)
                return RedirectToAction("Index", "Libro");

            var carrito = await ObtenerCarritoPorUsuarioAsync(idUsuario.Value);

            if (!carrito.Any())
            {
                TempData["Mensaje"] = "Tu carrito está vacío.";
                return RedirectToAction("Index");
            }

            var model = new ConfirmarCompraVista
            {
                IdUsuario = idUsuario.Value,
                Detalles = carrito
            };

            var metodos = await ObtenerMetodosDePago();
            ViewBag.MetodosDePago = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(metodos, "id_metodo_pago", "nombre");

            return View(model); 
        }



    }

}