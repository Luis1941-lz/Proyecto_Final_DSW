using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoApi_DSW.Data.Contrato;

namespace ProyectoApi_DSW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoUsuarioController : ControllerBase
    {

        private readonly ITipoUsuario tipoUsuarioDTA;

        public TipoUsuarioController(ITipoUsuario tipoUsuario)
        {
            tipoUsuarioDTA = tipoUsuario;
        }

        [HttpGet]

        public IActionResult Listar()
        {
            return Ok(tipoUsuarioDTA.listado());
        }
    }
}
