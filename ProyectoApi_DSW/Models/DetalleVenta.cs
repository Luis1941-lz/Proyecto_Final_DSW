namespace ProyectoApi_DSW.Models
{
    public class DetalleVenta
    {
        public int id_detalle { get; set; }
        public int id_venta { get; set; }
        public Venta Venta { get; set; }
        public int id_libro { get; set; }
        public Libro Libro { get; set; } 
        public int cantidad { get; set; }
    }
}
