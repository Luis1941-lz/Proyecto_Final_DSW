namespace ProyectoApi_DSW.Models.DTO
{
    public class VentaCompletaRequest
    {
        public int IdUsuario { get; set; }
        public int IdMetodoPago { get; set; }
        public DateTime FechaVenta { get; set; } = DateTime.Now;
        public List<DetalleVentaRequest> Detalles { get; set; } = new List<DetalleVentaRequest>();
    }


public class DetalleVentaItem
    {
        public int IdLibro { get; set; }
        public int Cantidad { get; set; } = 1;
    }
}
