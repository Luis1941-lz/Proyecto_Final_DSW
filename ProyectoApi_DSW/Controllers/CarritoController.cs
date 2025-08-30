using Microsoft.AspNetCore.Mvc;
using ProyectoApi_DSW.Data.Contrato;
using ProyectoApi_DSW.Models;
using ProyectoApi_DSW.Models.DTO;

namespace ProyectoApi_DSW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarritoController : ControllerBase
    {
        private readonly ICarrito carritoDTA;

        public CarritoController(ICarrito carrito)
        {
            carritoDTA = carrito;
        }

        // Obtener el carrito de un usuario
        [HttpGet("usuario/{idUsuario}")]
        public IActionResult ObtenerCarritoPorUsuario(int idUsuario)
        {
            var carrito = carritoDTA.ObtenerCarritoPorUsuario(idUsuario);
            if (carrito == null || !carrito.Any())
                return NotFound("No se encontró el carrito del usuario.");
            return Ok(carrito);
        }

        // Añadir un producto al carrito
        [HttpPost("agregar")]
        public IActionResult AgregarAlCarrito([FromBody] CarritoRequest request)
        {
            if (request == null)
                return BadRequest("Datos inválidos.");

            var carrito = new Carrito
            {
                IdUsuario = request.IdUsuario,
                IdLibro = request.IdLibro,
                Cantidad = request.Cantidad
            };

            var carritoActualizado = carritoDTA.AgregarAlCarrito(carrito);

            if (carritoActualizado == null)
                return BadRequest("No se pudo agregar el producto al carrito.");

            return Ok(carritoActualizado);
        }
        

        // Vaciar el carrito
        [HttpDelete("vaciar/{idUsuario}")]
        public IActionResult VaciarCarrito(int idUsuario)
        {
            bool vaciado = carritoDTA.VaciarCarrito(idUsuario);
            if (!vaciado)
                return NotFound("No se pudo vaciar el carrito.");
            return Ok("Carrito vaciado.");
        }
    }
}
