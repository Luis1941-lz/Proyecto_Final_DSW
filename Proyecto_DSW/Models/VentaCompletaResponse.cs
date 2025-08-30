namespace Proyecto_DSW.Models
{
    public class VentaCompletaResponse
    {
        public Venta Venta { get; set; }
        public List<DetalleVenta> Detalles { get; set; }
    }
}
