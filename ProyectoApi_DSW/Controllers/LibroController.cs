using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoApi_DSW.Data.Contrato;
using ProyectoApi_DSW.Models;

namespace ProyectoApi_DSW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibroController : ControllerBase
    {
        private readonly ILibro librosDTA;

        public LibroController(ILibro libro)
        {
            librosDTA = libro;
        }

        // GET: api/libro
        [HttpGet]
        public IActionResult Index()
        {
            var lista = librosDTA.ObtenerLibro();
            return Ok(lista);
        }

        // GET: api/libro/{id}
        [HttpGet("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var libro = librosDTA.ObtenerPorId(id);
            if (libro == null)
                return NotFound(new { mensaje = "Libro no encontrado" });

            return Ok(libro);
        }

        // POST: api/libro
        [HttpPost]
        public IActionResult Crear([FromBody] Libro libro)
        {
            if (libro == null)
                return BadRequest(new { mensaje = "Datos inválidos" });

            var nuevoLibro = librosDTA.Crear(libro);
            if (nuevoLibro == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "No se pudo crear el libro" });

            return Ok(nuevoLibro);
        }

        // PUT: api/libro/{id}
        [HttpPut("{id}")]
        public IActionResult Actualizar(int id, [FromBody] Libro libro)
        {
            if (libro == null || libro.id_libro != id)
                return BadRequest(new { mensaje = "Datos inválidos" });

            var actualizado = librosDTA.Actualizar(libro);
            if (actualizado == null)
                return NotFound(new { mensaje = "Libro no encontrado" });

            return Ok(actualizado);
        }

        // DELETE: api/libro/{id}
        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            var resultado = librosDTA.Eliminar(id);
            if (!resultado)
                return NotFound(new { mensaje = "Libro no encontrado o no se pudo eliminar" });

            return Ok(new { mensaje = "Libro eliminado correctamente" });
        }
    }
}
