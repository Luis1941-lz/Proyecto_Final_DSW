namespace ProyectoApi_DSW.Models
{
    public class Venta
    {
        public int id_venta { get; set; }
        public int id_usuario { get; set; }
        public Usuario Usuario { get; set; } 
        public int id_metodo_pago { get; set; }
        public MetodoPago MetodoPago { get; set; } 
        public DateTime fecha_venta { get; set; }

        public ICollection<DetalleVenta> DetallesVenta { get; set; }
    }
}
