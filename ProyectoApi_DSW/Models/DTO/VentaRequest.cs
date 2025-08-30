namespace ProyectoApi_DSW.Models.DTO
{
    public class VentaRequest
    {
        public int IdVenta { get; set; }         
        public int IdUsuario { get; set; }
        public int IdMetodoPago { get; set; }
        public DateTime FechaVenta { get; set; } = DateTime.Now;

    }
}
