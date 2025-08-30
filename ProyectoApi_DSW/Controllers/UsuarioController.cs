using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoApi_DSW.Data.Contrato;
using ProyectoApi_DSW.Models;

namespace ProyectoApi_DSW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarios usuariosDTA;

        public UsuarioController(IUsuarios usuario)
        {
            usuariosDTA = usuario;
        }

        [HttpGet]
        public IActionResult Listar()
        {
            return Ok(usuariosDTA.listado());
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var usuario = usuariosDTA.ObtenerPorId(id);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        [HttpPost]
        public IActionResult Crear([FromBody] Usuario usuario)
        {
            var creado = usuariosDTA.Crear(usuario);
            if (creado == null)
                return BadRequest("No se pudo crear el usuario.");

            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.id_usuario }, creado);
        }

        [HttpPut("{id}")]
        public IActionResult Actualizar(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.id_usuario)
                return BadRequest("El ID no coincide.");

            var actualizado = usuariosDTA.Actualizar(usuario);
            if (actualizado == null)
                return NotFound();

            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            bool eliminado = usuariosDTA.Eliminar(id);
            if (!eliminado)
                return NotFound();

            return NoContent();
        }

        [HttpPut("{id}/quitar-imagen")]
        public IActionResult QuitarImagen(int id)
        {
            bool resultado = usuariosDTA.QuitarImagen(id);

            if (!resultado)
                return NotFound(new { mensaje = "No se encontró el usuario o no se pudo quitar la imagen." });

            return Ok(new { mensaje = "Imagen eliminada correctamente." });
        }


        [HttpPost("expirar-suscripciones")]
        public IActionResult ExpirarSuscripciones()
        {
            usuariosDTA.ExpirarSuscripciones();
            return Ok("Suscripciones expiradas correctamente.");
        }
    }
}
