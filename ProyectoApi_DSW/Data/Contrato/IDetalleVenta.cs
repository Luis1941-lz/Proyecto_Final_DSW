using ProyectoApi_DSW.Models;

namespace ProyectoApi_DSW.Data.Contrato
{
    public interface IDetalleVenta
    {
        List<DetalleVenta> ObtenerDetallesVenta(int idVenta);
        DetalleVenta CrearDetalleVenta(DetalleVenta detalleVenta);
    }
}
