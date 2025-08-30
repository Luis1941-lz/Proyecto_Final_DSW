using Microsoft.AspNetCore.Mvc;
using ProyectoApi_DSW.Data.Contrato;
using ProyectoApi_DSW.Data.Repository;
using ProyectoApi_DSW.Models.DTO;

namespace ProyectoApi_DSW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUsuarios _usuariosDTA;

        public LoginController(IUsuarios usuariosDTA)
        {
            _usuariosDTA = usuariosDTA;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Correo) || string.IsNullOrEmpty(loginRequest.Contrasena))
            {
                return BadRequest("Correo y contraseña son requeridos.");
            }

            // 🔹 Ejecutar limpieza de suscripciones vencidas
            (_usuariosDTA as UsuarioRepository)?.ExpirarSuscripciones();

            var usuario = _usuariosDTA.ObtenerPorCorreoypassword(loginRequest.Correo, loginRequest.Contrasena);

            if (usuario == null)
            {
                return Unauthorized(new { mensaje = "Credenciales inválidas" });
            }

            return Ok(new
            {
                mensaje = "Login exitoso",
                usuario = new
                {
                    usuario.id_usuario,
                    usuario.nombre,
                    usuario.apellido,       
                    usuario.correo,
                    usuario.id_tipo_usuario,
                    usuario.id_estado,
                    usuario.imagen,
                }
            });
        }
    }
}
