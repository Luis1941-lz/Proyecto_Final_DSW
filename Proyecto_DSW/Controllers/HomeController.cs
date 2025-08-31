using Microsoft.AspNetCore.Mvc;
using Proyecto_DSW.Models;
using System.Diagnostics;

namespace Proyecto_DSW.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _config;

    

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;

            _config = config;
        }


        private async Task<List<Libro>> ObtenerLibrosDestacadosAsync()
        {
            using var http = new HttpClient();
            http.BaseAddress = new Uri(_config["Services:URL_API"]); // Asegúrate de tener esto en appsettings.json

            var response = await http.GetAsync("Libro"); // o la ruta correcta para obtener libros
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error al obtener libros: {response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync();
            var libros = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Libro>>(json);

            return libros.Take(3).ToList(); // Solo los primeros 3
        }


        public async Task<IActionResult> Index()
        {
            var librosDestacados = await ObtenerLibrosDestacadosAsync();
            return View(librosDestacados);
        }


        public IActionResult Privacy()
        {
            return View();
        }

            public IActionResult Contacto()
            {
                return View();
            }

            [HttpPost]
            public IActionResult EnviarMensaje([FromForm] ContactoViewModel model)
            {
                try
                {
                    // Aquí iría tu lógica para procesar el mensaje
                    // Por ejemplo: enviar email, guardar en base de datos, etc.

                    // Simulación de envío exitoso
                    return Json(new { success = true, message = "Mensaje enviado correctamente" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error al enviar el mensaje: " + ex.Message });
                }
            }
        

        public class ContactoViewModel
        {
            public string Nombre { get; set; }
            public string Email { get; set; }
            public string Telefono { get; set; }
            public string Asunto { get; set; }
            public string Mensaje { get; set; }
        }
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            public IActionResult Error()
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
