namespace Proyecto_DSW.Models
{
    public class Libro
    {

        public int id_libro { get; set; }
        public string titulo { get; set; }
        public string autor { get; set; }
        public decimal precio { get; set; }
        public int stock { get; set; }
        public string descripcion { get; set; }
        public bool tiene_pdf { get; set; }
        public string pdf_url { get; set; }
        public string imagen { get; set; }
        public int id_categoria { get; set; }
        public CategoriaLibro Categoria { get; set; }
    }
}
