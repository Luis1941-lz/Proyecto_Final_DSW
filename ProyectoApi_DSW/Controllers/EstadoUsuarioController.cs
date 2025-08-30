using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoApi_DSW.Data.Contrato;

namespace ProyectoApi_DSW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoUsuarioController : ControllerBase
    {
        private readonly IEstadoUsuario estadoUsuarioDTA;

        public EstadoUsuarioController(IEstadoUsuario estadoUsuario)
        {
            estadoUsuarioDTA = estadoUsuario;
        }

        [HttpGet]

        public IActionResult Listar()
        {
            return Ok(estadoUsuarioDTA.listado());
        }
    }
}
