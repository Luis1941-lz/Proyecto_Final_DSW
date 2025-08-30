using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Proyecto_DSW.Models.DTO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_DSW.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string contrasena)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(_config["Services:URL_API"]);

                    var loginData = new { Correo = correo, Contrasena = contrasena };
                    var json = JsonConvert.SerializeObject(loginData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync("Login/login", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(result);

                        if (loginResponse?.usuario != null)
                        {
                            var usuario = loginResponse.usuario;


                            HttpContext.Session.SetString("usuario_nombre", $"{usuario.nombre} {usuario.apellido}");
                            HttpContext.Session.SetInt32("usuario_id", usuario.id_usuario);
                            HttpContext.Session.SetInt32("usuario_tipo", usuario.id_tipo_usuario);
                            HttpContext.Session.SetInt32("usuario_estado", usuario.id_estado);


                            HttpContext.Session.SetString("usuario_imagen", usuario.imagen ?? "");

                            return RedirectToAction("Index", "Home");
                        }

                        else
                        {
                            ViewBag.Error = "Credenciales incorrectas.";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.Error = $"Error en el servidor: {response.StatusCode}";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Excepción: {ex.Message}";
                return View();
            }


        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); 
            return RedirectToAction("Login", "Login"); 
        }
    }
}
