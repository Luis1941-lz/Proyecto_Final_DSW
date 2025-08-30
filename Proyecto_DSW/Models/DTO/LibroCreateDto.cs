using Microsoft.AspNetCore.Http;

namespace Proyecto_DSW.Models.DTO
{
    public class LibroCreateDto
    {
        public int id_libro { get; set; } = 0;
        public string titulo { get; set; }
        public string autor { get; set; }
        public decimal precio { get; set; }
        public int stock { get; set; }
        public string descripcion { get; set; }

        public bool tiene_pdf { get; set; } = false;
        public string pdf_url { get; set; } = "";
        public string imagen { get; set; } = "";

        public int id_categoria { get; set; }

        // Para desplegar la lista de categorías en la vista
        public string nombre_categoria { get; set; } = "";

        // Archivos subidos desde la vista
        [System.Text.Json.Serialization.JsonIgnore]
        public IFormFile Portada { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public IFormFile Pdf { get; set; }
    }
}
