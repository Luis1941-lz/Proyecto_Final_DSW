using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoApi_DSW.Data.Contrato;
using ProyectoApi_DSW.Models;
using ProyectoApi_DSW.Models.DTO;
using System;
using System.Linq;

namespace ProyectoApi_DSW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagoController : ControllerBase
    {
        private readonly IPago pagoDTA;

        public PagoController(IPago pago)
        {
            pagoDTA = pago;
        }

        [HttpPost("realizar-pago")]
        public IActionResult RealizarPago([FromBody] PagoRequest request)
        {
            if (request == null)
                return BadRequest("Los datos del pago no son válidos.");

            if (request.monto <= 0)
                return BadRequest("El monto debe ser mayor que cero.");

            if (request.id_usuario <= 0 || request.id_metodo_pago <= 0)
                return BadRequest("El usuario o el método de pago no son válidos.");

            // Validar suscripción si viene
            if (request.Suscripcion != null)
            {
                if (request.Suscripcion.fecha_inicio == default)
                    return BadRequest("La fecha de inicio de la suscripción es obligatoria.");
            }

            try
            {

                var pago = new Pago
                {
                    id_usuario = request.id_usuario,
                    id_metodo_pago = request.id_metodo_pago,
                    monto = request.monto,
                    estado = request.estado,
                    Suscripcion = request.Suscripcion != null
                        ? new Suscripcion
                        {
                            id_plan = request.Suscripcion.id_plan,
                            fecha_inicio = request.Suscripcion.fecha_inicio
                        }
                        : null
                };

                var resultado = pagoDTA.RealizarPago(pago);

                if (resultado == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error al procesar el pago.");

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("{id_usuario}")]
        public IActionResult ObtenerPagosPorUsuario(int id_usuario)
        {
            try
            {
                var pagos = pagoDTA.ObtenerPagosPorUsuario(id_usuario);

                if (pagos == null || !pagos.Any())
                    return NotFound("No se encontraron pagos para este usuario.");

                return Ok(pagos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener los pagos: {ex.Message}");
            }
        }
    }
}
