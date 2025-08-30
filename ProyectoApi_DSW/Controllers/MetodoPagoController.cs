using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoApi_DSW.Data.Contrato;

namespace ProyectoApi_DSW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetodoPagoController : ControllerBase
    {

        private readonly IMetodoPago _metodoPagoDTA;

        public MetodoPagoController(IMetodoPago metodoPagoDTA)
        {
            _metodoPagoDTA = metodoPagoDTA;
        }

        [HttpGet]
        public IActionResult listar()
        {
            return Ok(_metodoPagoDTA.listarMetodoPago());
        }
    }
}
