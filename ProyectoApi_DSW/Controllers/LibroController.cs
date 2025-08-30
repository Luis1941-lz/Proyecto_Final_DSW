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

        [HttpGet]
        public IActionResult Index()
        {
            return Ok(librosDTA.ObtenerLibro());
        }

        [HttpPost]
        public IActionResult Crear(Libro libro)
        {
            var nuevoLibro = librosDTA.Crear(libro);
            if (nuevoLibro == null)
                return NotFound();
            return Ok(nuevoLibro);
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var libro = librosDTA.ObtenerPorId(id);
            if (libro == null)
                return NotFound();  
            return Ok(libro);  
        }
    }
}
