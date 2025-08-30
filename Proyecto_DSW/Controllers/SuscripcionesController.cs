using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Proyecto_DSW.Models;

namespace Proyecto_DSW.Controllers
{
    public class SuscripcionesController : Controller
    {
        private readonly IConfiguration _config;

        public SuscripcionesController(IConfiguration config)
        {
            _config = config;
        }

        private async Task<List<PlanSuscripciones>> obtenerListadoSuscripciones()
        {
            var listado = new List<PlanSuscripciones>();

            using (var suscripcionHttp = new HttpClient())
            {
                suscripcionHttp.BaseAddress = new Uri(_config["Services:URL_API"]);

                var mensaje = await suscripcionHttp.GetAsync("Suscripcion");

                var data = await mensaje.Content.ReadAsStringAsync();

                 listado = JsonConvert.DeserializeObject<List<PlanSuscripciones>>(data);
                
            }
            return listado;
        }

        public IActionResult Index()
        {
            int? estado = HttpContext.Session.GetInt32("usuario_estado");

            if (estado != 1) // si no está activo
            {
                TempData["AccesoDenegado"] = "Actualmente no puedes acceder a los planes de suscripción. Tu cuenta no está activa.";
                return RedirectToAction("Index", "Home"); 
            }

            var listado = obtenerListadoSuscripciones().Result;
            return View(listado);
        }

    }
}
