using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Proyecto_DSW.Models;
using Proyecto_DSW.Models.DTO;

namespace Proyecto_DSW.Controllers
{
    public class LibroController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public LibroController(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        private Libro obtenerLibroPorID(int id)
        {
            using var http = new HttpClient();
            http.BaseAddress = new Uri(_config["Services:URL_API"]);
            var mensaje = http.GetAsync($"Libro/{id}").Result;

            if (!mensaje.IsSuccessStatusCode)
                throw new Exception($"Error API: {mensaje.StatusCode}");

            var data = mensaje.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<Libro>(data);
        }

        private Libro registrarLibro(Libro libro)
        {
            using var http = new HttpClient();
            http.BaseAddress = new Uri(_config["Services:URL_API"]);

            var contenido = new StringContent(
                JsonConvert.SerializeObject(libro),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var mensaje = http.PostAsync("Libro", contenido).Result;

            if (!mensaje.IsSuccessStatusCode)
                throw new Exception($"Error API: {mensaje.StatusCode}");

            var data = mensaje.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<Libro>(data);
        }

        private List<CategoriaLibro> obtenerCategorias()
        {
            using var http = new HttpClient();
            http.BaseAddress = new Uri(_config["Services:URL_API"]);
            var mensaje = http.GetAsync("CategoriaLibro").Result;

            if (!mensaje.IsSuccessStatusCode)
                throw new Exception($"Error API Categorías: {mensaje.StatusCode}");

            var data = mensaje.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<List<CategoriaLibro>>(data);
        }

        private async Task<List<Libro>> obtenerListadoLibro()
        {
            var listado = new List<Libro>();

            using (var libroHttp = new HttpClient())
            {
                libroHttp.BaseAddress = new Uri(_config["Services:URL_API"]);

                var mensaje = await libroHttp.GetAsync("Libro");

                var data = await mensaje.Content.ReadAsStringAsync();

                listado = JsonConvert.DeserializeObject<List<Libro>>(data);

                foreach (var l in listado)
                {
                    if (l.Categoria == null)
                    {
                        l.Categoria = new CategoriaLibro
                        {
                            id_categoria = l.id_categoria,
                            nombre = "Sin categoría" 
                        };
                    }
                }
            }
            return listado;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var listado = obtenerListadoLibro().Result;
            return View(listado);
        }

        [HttpGet]
        public IActionResult ListadoLibros()
        {
            var listado = obtenerListadoLibro().Result;
            return View(listado);
        }

        public IActionResult Create()
        {
            var dto = new LibroCreateDto();
            try
            {
                ViewBag.Categorias = new SelectList(obtenerCategorias(), "id_categoria", "nombre");
            }
            catch
            {
                ViewBag.Categorias = new List<SelectListItem>();
            }
            return View(dto);
        }

        [HttpPost]
        public IActionResult Create(LibroCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categorias = new SelectList(obtenerCategorias(), "id_categoria", "nombre");
                    return View(dto);
                }

                long maxImagenSize = 10 * 1024 * 1024;   
                long maxPdfSize = 100 * 1024 * 1024;     


                if (dto.Portada != null && dto.Portada.Length > 0)
                {
                    var extImg = Path.GetExtension(dto.Portada.FileName).ToLower();
                    if (extImg != ".jpg" && extImg != ".jpeg" && extImg != ".png" && extImg != ".gif")
                        throw new Exception("Solo se permiten imágenes (.jpg, .jpeg, .png, .gif)");

                    if (dto.Portada.Length > maxImagenSize)
                        throw new Exception("La imagen excede el tamaño máximo (10 MB)");

                    var uploadsPath = Path.Combine(_env.WebRootPath, "imagenes");
                    if (!Directory.Exists(uploadsPath))
                        Directory.CreateDirectory(uploadsPath);

                    var fileName = Guid.NewGuid() + extImg;
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        dto.Portada.CopyTo(stream);

                    dto.imagen = "/imagenes/" + fileName;
                }


                if (dto.Pdf != null && dto.Pdf.Length > 0)
                {
                    var extPdf = Path.GetExtension(dto.Pdf.FileName).ToLower();
                    if (extPdf != ".pdf")
                        throw new Exception("Solo se permite archivo PDF");

                    if (dto.Pdf.Length > maxPdfSize)
                        throw new Exception("El PDF excede el tamaño máximo (100 MB)");

                    var pdfPath = Path.Combine(_env.WebRootPath, "pdfs");
                    if (!Directory.Exists(pdfPath))
                        Directory.CreateDirectory(pdfPath);

                    var fileName = Guid.NewGuid() + extPdf;
                    var filePath = Path.Combine(pdfPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        dto.Pdf.CopyTo(stream);

                    dto.pdf_url = "/pdfs/" + fileName;
                    dto.tiene_pdf = true;
                }

                var libro = new Libro
                {
                    id_libro = 0,
                    titulo = dto.titulo,
                    autor = dto.autor,
                    precio = dto.precio,
                    stock = dto.stock,
                    descripcion = dto.descripcion,
                    tiene_pdf = dto.tiene_pdf,
                    pdf_url = dto.pdf_url ?? "",
                    imagen = dto.imagen ?? "",
                    id_categoria = dto.id_categoria,
                    Categoria = new CategoriaLibro
                    {
                        id_categoria = dto.id_categoria,
                        nombre = dto.nombre_categoria ?? ""
                    }
                };

                var nuevoLibro = registrarLibro(libro);
                return RedirectToAction("Details", new { id = nuevoLibro.id_libro });
            }
            catch (Exception ex)
            {
                ViewBag.Categorias = new SelectList(obtenerCategorias(), "id_categoria", "nombre");
                ModelState.AddModelError("", "Error registrando libro: " + ex.Message);
                return View(dto);
            }
        }

        public IActionResult Details(int id)
        {
            int? estado = HttpContext.Session.GetInt32("usuario_estado");

            int usuarioActivo = 1; 

            if (estado != usuarioActivo)
            {
                TempData["AccesoDenegado"] = "Actualmente no puedes acceder a esta opción.";
                return RedirectToAction("Index");
            }

            try
            {
                var libro = obtenerLibroPorID(id);


                ViewBag.IdUsuario = HttpContext.Session.GetInt32("usuario_id") ?? 1;

                return View(libro);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error cargando detalles: " + ex.Message);
                return RedirectToAction("Index");
            }
        }


    }
}
