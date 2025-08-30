using Microsoft.AspNetCore.Mvc;
using ProyectoApi_DSW.Data.Contrato;
using ProyectoApi_DSW.Models;
using ProyectoApi_DSW.Models.DTO;

namespace ProyectoApi_DSW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentaController : ControllerBase
    {
        private readonly IVenta ventaDTA;
        private readonly IDetalleVenta detalleVentaDTA;

        public VentaController(IVenta venta, IDetalleVenta detalleVenta)
        {
            ventaDTA = venta;
            detalleVentaDTA = detalleVenta;
        }

        [HttpPost("crear-completa")]
        public IActionResult CrearVentaCompleta([FromBody] VentaCompletaRequest request)
        {
            if (request == null || request.Detalles == null || !request.Detalles.Any())
                return BadRequest("Datos inválidos.");

            try
            {

                var venta = new Venta
                {
                    id_usuario = request.IdUsuario,
                    id_metodo_pago = request.IdMetodoPago,
                    fecha_venta = request.FechaVenta
                };

                var nuevaVenta = ventaDTA.CrearVenta(venta);


                foreach (var item in request.Detalles)
                {
                    var detalle = new DetalleVenta
                    {
                        id_venta = nuevaVenta.id_venta,
                        id_libro = item.IdLibro,
                        cantidad = item.Cantidad
                    };
                    detalleVentaDTA.CrearDetalleVenta(detalle);
                }

                var detalles = detalleVentaDTA.ObtenerDetallesVenta(nuevaVenta.id_venta);

                var resultado = new VentaCompletaResponse
                {
                    Venta = nuevaVenta,
                    Detalles = detalles
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear la venta completa: " + ex.Message);
            }
        }


        [HttpGet("usuario/{idUsuario}")]
        public IActionResult ObtenerVentasPorUsuario(int idUsuario)
        {
            var ventas = ventaDTA.ObtenerVentasPorUsuario(idUsuario);


            if (ventas == null || !ventas.Any())
                return Ok(new List<Venta>());

            return Ok(ventas);
        }



        [HttpGet("{idVenta}")]
        public IActionResult ObtenerVentaPorId(int idVenta)
        {
            var venta = ventaDTA.ObtenerVentaPorId(idVenta);
            if (venta == null)
                return NotFound("Venta no encontrada.");
            return Ok(venta);
        }


        [HttpPut("actualizar")]
        public IActionResult ActualizarVenta([FromBody] VentaRequest request)
        {
            if (request == null)
                return BadRequest("Datos inválidos.");

            var venta = new Venta
            {
                id_venta = request.IdVenta,
                id_usuario = request.IdUsuario,
                id_metodo_pago = request.IdMetodoPago,
                fecha_venta = request.FechaVenta
            };

            var ventaActualizada = ventaDTA.ActualizarVenta(venta);
            return Ok(ventaActualizada);
        }


        [HttpDelete("{idVenta}")]
        public IActionResult EliminarVenta(int idVenta)
        {
            bool eliminado = ventaDTA.EliminarVenta(idVenta);
            if (!eliminado)
                return NotFound("No se pudo eliminar la venta.");
            return Ok("Venta eliminada correctamente.");
        }
    }
}
