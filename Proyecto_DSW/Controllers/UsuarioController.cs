using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Proyecto_DSW.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_DSW.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IConfiguration _config;

        public UsuarioController(IConfiguration config)
        {
            _config = config;
        }

        private async Task<List<Usuario>> ObtenerListadoAsync()
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_config["Services:URL_API"]);

            var response = await httpClient.GetAsync("Usuario");
            if (!response.IsSuccessStatusCode)
                return new List<Usuario>();

            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Usuario>>(data);
        }

        private async Task<Usuario> ObtenerUsuarioPorIdAsync(int id)
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_config["Services:URL_API"]);

            var response = await httpClient.GetAsync($"Usuario/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Usuario>(data);
        }

        private async Task<List<TipoUsuario>> ObtenerTiposAsync()
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_config["Services:URL_API"]);

            var response = await httpClient.GetAsync("TipoUsuario");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener tipos de usuario");

            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<TipoUsuario>>(data);
        }

        private async Task<List<Estado>> ObtenerEstadosAsync()
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_config["Services:URL_API"]);

            var response = await httpClient.GetAsync("EstadoUsuario");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener estados");

            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Estado>>(data);
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listado = await ObtenerListadoAsync();
            var tipos = new List<SelectListItem>();

            try
            {
                var tiposUsuario = await ObtenerTiposAsync();
                tipos = tiposUsuario.Select(t => new SelectListItem
                {
                    Value = t.id_tipo_usuario.ToString(),
                    Text = t.nombre
                }).ToList();
            }
            catch
            {
                // En caso de error, lista vacía
                tipos = new List<SelectListItem>();
            }

            ViewBag.Tipos = tipos;

            return View(listado);
        }


        [HttpGet]
        public async Task<IActionResult> Detalles(int id)
        {
            var usuario = await ObtenerUsuarioPorIdAsync(id);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            try
            {
                ViewBag.Tipos = new SelectList(await ObtenerTiposAsync(), "id_tipo_usuario", "nombre");
                ViewBag.Estados = new SelectList(await ObtenerEstadosAsync(), "id_estado", "nombre");
            }
            catch
            {
                ViewBag.Tipos = new List<SelectListItem>();
                ViewBag.Estados = new List<SelectListItem>();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Tipos = new SelectList(await ObtenerTiposAsync(), "id_tipo_usuario", "nombre");
                ViewBag.Estados = new SelectList(await ObtenerEstadosAsync(), "id_estado", "nombre");
                return View(usuario);
            }

            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_config["Services:URL_API"]);

            var json = JsonConvert.SerializeObject(usuario);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("Usuario", content);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "No se pudo crear el usuario.");
                ViewBag.Tipos = new SelectList(await ObtenerTiposAsync(), "id_tipo_usuario", "nombre");
                ViewBag.Estados = new SelectList(await ObtenerEstadosAsync(), "id_estado", "nombre");
                return View(usuario);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DetalleAdmin(int id)
        {
            var usuario = await ObtenerUsuarioPorIdAsync(id);
            if (usuario == null)
                return NotFound();

            return View(usuario); // Vista: DetalleAdmin.cshtml
        }


        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuario = await ObtenerUsuarioPorIdAsync(id);
            if (usuario == null)
                return NotFound();

            try
            {
                ViewBag.Tipos = new SelectList(await ObtenerTiposAsync(), "id_tipo_usuario", "nombre", usuario.id_tipo_usuario);
                ViewBag.Estados = new SelectList(await ObtenerEstadosAsync(), "id_estado", "nombre", usuario.id_estado);
            }
            catch
            {
                ViewBag.Tipos = new List<SelectListItem>();
                ViewBag.Estados = new List<SelectListItem>();
            }

            return View(usuario);
        }

        [HttpGet]
        public async Task<IActionResult> EditarUsuario(int id)
        {
            var usuario = await ObtenerUsuarioPorIdAsync(id);
            if (usuario == null)
                return NotFound();

            try
            {
                ViewBag.Tipos = new SelectList(await ObtenerTiposAsync(), "id_tipo_usuario", "nombre", usuario.id_tipo_usuario);
                ViewBag.Estados = new SelectList(await ObtenerEstadosAsync(), "id_estado", "nombre", usuario.id_estado);
            }
            catch
            {
                ViewBag.Tipos = new List<SelectListItem>();
                ViewBag.Estados = new List<SelectListItem>();
            }

            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, Usuario usuario)
        {
            if (id != usuario.id_usuario)
                return BadRequest("El ID no coincide.");

            if (!ModelState.IsValid)
            {
                var errores = ModelState.SelectMany(kvp => kvp.Value.Errors.Select(error => new
                {
                    Campo = kvp.Key,
                    Error = error.ErrorMessage
                })).ToList();


                foreach (var e in errores)
                {
                    Console.WriteLine($"Campo: {e.Campo}, Error: {e.Error}");
                }

                // Re-cargar SelectLists antes de retornar la vista
                ViewBag.Tipos = new SelectList(await ObtenerTiposAsync(), "id_tipo_usuario", "nombre", usuario.id_tipo_usuario);
                ViewBag.Estados = new SelectList(await ObtenerEstadosAsync(), "id_estado", "nombre", usuario.id_estado);

                return View(usuario);
            }


            // Serializamos directamente el objeto usuario
            var json = JsonConvert.SerializeObject(usuario);
            Console.WriteLine("JSON enviado al API:");
            Console.WriteLine(json); // Te ayuda a depurar el contenido exacto

            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_config["Services:URL_API"]);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync($"Usuario/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                // Opcional: Leer contenido del error
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error al actualizar usuario: {response.StatusCode}");
                Console.WriteLine(errorContent);

                ModelState.AddModelError("", "Error al actualizar el usuario.");
                ViewBag.Tipos = new SelectList(await ObtenerTiposAsync(), "id_tipo_usuario", "nombre", usuario.id_tipo_usuario);
                ViewBag.Estados = new SelectList(await ObtenerEstadosAsync(), "id_estado", "nombre", usuario.id_estado);
                return View(usuario);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> EditarUsuario(int id, Usuario usuario, IFormFile? nuevaImagen, bool quitarImagen)
        {
            if (id != usuario.id_usuario)
                return BadRequest("El ID no coincide.");

            if (!ModelState.IsValid)
            {
                // 🔁 Recargar SelectLists
                ViewBag.Tipos = new SelectList(await ObtenerTiposAsync(), "id_tipo_usuario", "nombre", usuario.id_tipo_usuario);
                ViewBag.Estados = new SelectList(await ObtenerEstadosAsync(), "id_estado", "nombre", usuario.id_estado);
                return View(usuario);
            }

            // ✅ Manejo de la imagen
            if (quitarImagen)
            {
                // Eliminar imagen → se manda null al API
                usuario.imagen = null;
            }
            else if (nuevaImagen != null && nuevaImagen.Length > 0)
            {
                // Validar extensión
                var ext = Path.GetExtension(nuevaImagen.FileName).ToLower();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".gif")
                {
                    ModelState.AddModelError("imagen", "Solo se permiten imágenes jpg, jpeg, png o gif.");
                    ViewBag.Tipos = new SelectList(await ObtenerTiposAsync(), "id_tipo_usuario", "nombre", usuario.id_tipo_usuario);
                    ViewBag.Estados = new SelectList(await ObtenerEstadosAsync(), "id_estado", "nombre", usuario.id_estado);
                    return View(usuario);
                }

                // 📁 Guardar en wwwroot/imagenes
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagenes");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid() + ext;
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await nuevaImagen.CopyToAsync(stream);

                usuario.imagen = "/imagenes/" + fileName;
            }
            else
            {
                // Si no subió nada → mantener imagen anterior
                var usuarioExistente = await ObtenerUsuarioPorIdAsync(id);
                if (usuarioExistente != null)
                    usuario.imagen = usuarioExistente.imagen;
            }

            // 🔄 PUT al API
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_config["Services:URL_API"]);

            var json = JsonConvert.SerializeObject(usuario);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync($"Usuario/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Error al actualizar usuario.");
                ViewBag.Tipos = new SelectList(await ObtenerTiposAsync(), "id_tipo_usuario", "nombre", usuario.id_tipo_usuario);
                ViewBag.Estados = new SelectList(await ObtenerEstadosAsync(), "id_estado", "nombre", usuario.id_estado);
                return View(usuario);
            }

            // 🧠 Actualiza sesión
            HttpContext.Session.SetString("usuario_nombre", $"{usuario.nombre} {usuario.apellido}");
            HttpContext.Session.SetString("usuario_imagen", usuario.imagen ?? "");

            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public async Task<IActionResult> QuitarImagen(int id)
        {
            // Obtener usuario actual
            var usuario = await ObtenerUsuarioPorIdAsync(id);
            if (usuario == null)
                return NotFound();

            // 🗑️ Eliminar archivo físico si existe
            if (!string.IsNullOrEmpty(usuario.imagen))
            {
                var rutaFisica = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", usuario.imagen.TrimStart('/'));
                if (System.IO.File.Exists(rutaFisica))
                    System.IO.File.Delete(rutaFisica);
            }

            // Actualizar usuario con imagen = null
            usuario.imagen = null;

            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_config["Services:URL_API"]);

            var json = JsonConvert.SerializeObject(usuario);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync($"Usuario/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Error al quitar la imagen.");
                return RedirectToAction("EditarUsuario", new { id });
            }

            // 🔄 Actualizar sesión también
            HttpContext.Session.SetString("usuario_imagen", "");

            // ✅ Volver a la vista de edición
            return RedirectToAction("EditarUsuario", new { id });
        }





        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_config["Services:URL_API"]);

            var response = await httpClient.DeleteAsync($"Usuario/{id}");
            if (!response.IsSuccessStatusCode)
            {
                // ❌ Si falla → redirige sin TempData
                return RedirectToAction(nameof(Index));
            }

            // ✅ Eliminado correctamente → redirige sin mensajes
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public IActionResult Registro()
        {
            return View(); // Muestra el formulario de registro
        }

        [HttpPost]
        public async Task<IActionResult> Registro(Usuario usuario)
        {
            // ⚡ Valores por defecto
            usuario.id_tipo_usuario = 2; // Usuario normal
            usuario.id_estado = 1;       // Activo

            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_config["Services:URL_API"]);

            var json = JsonConvert.SerializeObject(usuario);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("Usuario", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "❌ No se pudo registrar el usuario.");
                return View(usuario);
            }

            // ✅ Si el registro fue exitoso → redirige a Login
            return RedirectToAction("Login", "Login");
            // 👆 Cambia "Auth" por el controlador donde está tu Login
        }


    }
}
