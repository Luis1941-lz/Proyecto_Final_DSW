namespace ProyectoApi_DSW.Models.DTO
{
    public class DetalleVentaRequest
    {

        public int IdDetalle { get; set; }       // Opcional, si se desea actualizar
        public int IdVenta { get; set; }
        public int IdLibro { get; set; }
        public int Cantidad { get; set; } = 1;
    }
}