namespace ProyectoApi_DSW.Models.DTO
{
    public class VentaCompletaResponse
    {
        public Venta Venta { get; set; }
        public List<DetalleVenta> Detalles { get; set; }
    }
}
