namespace Proyecto_DSW.Models
{
    public class VentaCompletaRequest
    {
        public int IdUsuario { get; set; }
        public int IdMetodoPago { get; set; }
        public DateTime FechaVenta { get; set; }
        public List<DetalleVentaRequest> Detalles { get; set; }

    }
}
