using ProyectoApi_DSW.Models;

namespace ProyectoApi_DSW.Data.Contrato
{
    public interface IVenta
    {
        Venta CrearVenta(Venta venta); 
        List<Venta> ObtenerVentasPorUsuario(int idUsuario);  
        Venta ObtenerVentaPorId(int idVenta);  
        List<Venta> ObtenerTodasLasVentas();  
        bool EliminarVenta(int idVenta);  
        Venta ActualizarVenta(Venta venta);  
    }
}
