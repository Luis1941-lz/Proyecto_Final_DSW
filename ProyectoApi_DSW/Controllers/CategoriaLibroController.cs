using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoApi_DSW.Data.Contrato;

namespace ProyectoApi_DSW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaLibroController : ControllerBase
    {
        private readonly ICategoriaLibro _categoriaLibroRepository;


        public CategoriaLibroController(ICategoriaLibro categoriaLibroRepository)
        {
            _categoriaLibroRepository = categoriaLibroRepository;
        }
        [HttpGet]
        public IActionResult GetCategorias()
        {
            var categorias = _categoriaLibroRepository.obtenerCategoria();
            if (categorias == null || !categorias.Any())
            {
                return NotFound("No se encontraron categorías.");
            }
            return Ok(categorias);
        }
    }
}
