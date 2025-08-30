using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoApi_DSW.Data.Contrato;
using ProyectoApi_DSW.Models;

namespace ProyectoApi_DSW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuscripcionController : ControllerBase
    {
        private readonly ISuscripciones _suscripcionesRepository;  

        public SuscripcionController(ISuscripciones suscripcionesRepository)
        {
            _suscripcionesRepository = suscripcionesRepository;
        }

   
        [HttpGet]
        public IActionResult Index()
        {
            var suscripciones = _suscripcionesRepository.ObtenerPlanes();
            return Ok(suscripciones);
        }


        [HttpGet("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var plan = _suscripcionesRepository.ObtenerPlanPorId(id);

            if (plan == null)
            {
                return NotFound($"Suscripción con id {id} no encontrada.");
            }

            return Ok(plan);
        }
    }
}
