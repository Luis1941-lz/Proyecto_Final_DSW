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

        // =========================
        // 🔹 Métodos privados
        // =========================

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

        private Libro actualizarLibro(int id, Libro libro)
        {
            using var http = new HttpClient();
            http.BaseAddress = new Uri(_config["Services:URL_API"]);

            var contenido = new StringContent(
                JsonConvert.SerializeObject(libro),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var mensaje = http.PutAsync($"Libro/{id}", contenido).Result;

            if (!mensaje.IsSuccessStatusCode)
                throw new Exception($"Error API: {mensaje.StatusCode}");

            var data = mensaje.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<Libro>(data);
        }

        private bool eliminarLibro(int id)
        {
            using var http = new HttpClient();
            http.BaseAddress = new Uri(_config["Services:URL_API"]);
            var mensaje = http.DeleteAsync($"Libro/{id}").Result;

            return mensaje.IsSuccessStatusCode;
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

        // =========================
        // 🔹 Métodos públicos (acciones)
        // =========================

        [HttpGet]
        public IActionResult Index(string? busqueda, int? categoriaId, int pagina = 1, int porPagina = 6)
        {
            var listado = obtenerListadoLibro().Result;

            if (!string.IsNullOrEmpty(busqueda))
                listado = listado.Where(l =>
                    l.titulo.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                    l.autor.Contains(busqueda, StringComparison.OrdinalIgnoreCase)).ToList();

            if (categoriaId.HasValue && categoriaId.Value > 0)
                listado = listado.Where(l => l.id_categoria == categoriaId.Value).ToList();

            int totalRegistros = listado.Count;
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / porPagina);
            var listadoPagina = listado.Skip((pagina - 1) * porPagina).Take(porPagina).ToList();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.BusquedaActual = busqueda;
            ViewBag.CategoriaSeleccionada = categoriaId;
            ViewBag.Categorias = new SelectList(obtenerCategorias(), "id_categoria", "nombre");

            return View(listadoPagina);
        }

        [HttpGet]
        public IActionResult ListadoLibros(string? busqueda, int? categoriaId, int pagina = 1, int porPagina = 6)
        {
            var libros = obtenerListadoLibro().Result;

            if (!string.IsNullOrEmpty(busqueda))
            {
                libros = libros.Where(l =>
                    l.titulo.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                    l.autor.Contains(busqueda, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (categoriaId.HasValue && categoriaId.Value > 0)
            {
                libros = libros.Where(l => l.id_categoria == categoriaId.Value).ToList();
            }

            int totalRegistros = libros.Count;
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / porPagina);
            var librosPagina = libros.Skip((pagina - 1) * porPagina).Take(porPagina).ToList();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.BusquedaActual = busqueda;
            ViewBag.CategoriaSeleccionada = categoriaId;
            ViewBag.Categorias = new SelectList(obtenerCategorias(), "id_categoria", "nombre");

            return View(librosPagina);
        }

        // GET: Libro/Create
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

        // POST: Libro/Create
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

                long maxImagenSize = 10 * 1024 * 1024;   // 10 MB
                long maxPdfSize = 100 * 1024 * 1024;     // 100 MB

                // =======================
                // Guardar imagen
                // =======================
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

                // =======================
                // Guardar PDF
                // =======================
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

                // =======================
                // Mapear DTO a Modelo API
                // =======================
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

        // GET: Libro/Edit/{id}
        public IActionResult Edit(int id)
        {
            try
            {
                var libro = obtenerLibroPorID(id);

                var dto = new LibroCreateDto
                {
                    id_libro = libro.id_libro,
                    titulo = libro.titulo,
                    autor = libro.autor,
                    precio = libro.precio,
                    stock = libro.stock,
                    descripcion = libro.descripcion,
                    tiene_pdf = libro.tiene_pdf,
                    pdf_url = libro.pdf_url,
                    imagen = libro.imagen,
                    id_categoria = libro.id_categoria,
                    nombre_categoria = libro.Categoria?.nombre ?? ""
                };

                ViewBag.Categorias = new SelectList(obtenerCategorias(), "id_categoria", "nombre", libro.id_categoria);

                return View(dto); // ✅ ahora la vista recibe el tipo correcto
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error cargando libro: " + ex.Message;
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public IActionResult Edit(int id, LibroCreateDto dto)
        {
            try
            {
                // ⚡ Eliminar validaciones innecesarias en edición
                ModelState.Remove("imagen");
                ModelState.Remove("pdf_url");
                ModelState.Remove("nombre_categoria");
                ModelState.Remove("Portada");
                ModelState.Remove("Pdf");

                if (!ModelState.IsValid)
                {
                    ViewBag.Categorias = new SelectList(obtenerCategorias(), "id_categoria", "nombre", dto.id_categoria);
                    return View(dto);
                }

                // 🔎 Asegurar que el id coincida con la URL
                if (dto.id_libro != id)
                    dto.id_libro = id;

                long maxImagenSize = 10 * 1024 * 1024;  // 10 MB
                long maxPdfSize = 100 * 1024 * 1024;    // 100 MB

                // 📌 Obtener el libro actual para no perder datos previos
                var libroExistente = obtenerLibroPorID(id);

                // 📂 Imagen
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
                else
                {
                    // 🔹 Mantener imagen anterior
                    dto.imagen = libroExistente.imagen;
                }

                // 📂 PDF
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
                else
                {
                    // 🔹 Mantener PDF anterior
                    dto.pdf_url = libroExistente.pdf_url;
                    dto.tiene_pdf = libroExistente.tiene_pdf;
                }

                // 📌 Mapear al modelo Libro final
                var libro = new Libro
                {
                    id_libro = id,   // ⚡ fuerza el ID correcto
                    titulo = dto.titulo,
                    autor = dto.autor,
                    precio = dto.precio,
                    stock = dto.stock,
                    descripcion = dto.descripcion,
                    tiene_pdf = dto.tiene_pdf,
                    pdf_url = string.IsNullOrEmpty(dto.pdf_url) ? null : dto.pdf_url,
                    imagen = string.IsNullOrEmpty(dto.imagen) ? null : dto.imagen,
                    id_categoria = dto.id_categoria
                };


                // 🔄 Llamar API para actualizar
                var actualizado = actualizarLibro(id, libro);

                return RedirectToAction("Details", new { id = actualizado.id_libro });
            }
            catch (Exception ex)
            {
                ViewBag.Categorias = new SelectList(obtenerCategorias(), "id_categoria", "nombre", dto.id_categoria);
                ModelState.AddModelError("", "Error actualizando libro: " + ex.Message);
                return View(dto);
            }
        }




        // GET: Libro/Delete/{id}
        public IActionResult Delete(int id)
        {
            try
            {
                var libro = obtenerLibroPorID(id);
                return View(libro);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error cargando libro: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: Libro/Delete/{id}
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var ok = eliminarLibro(id);
                if (!ok)
                    TempData["Error"] = "No se pudo eliminar el libro.";
                else
                    TempData["Mensaje"] = "Libro eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error eliminando libro: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            try
            {
                var libro = obtenerLibroPorID(id);
                return View(libro);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error cargando detalles: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
