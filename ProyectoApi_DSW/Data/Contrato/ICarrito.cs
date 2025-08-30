using ProyectoApi_DSW.Models;

namespace ProyectoApi_DSW.Data.Contrato
{
    public interface ICarrito
    {
        List<Carrito> ObtenerCarritoPorUsuario(int idUsuario);
        Carrito ObtenerPorId(int idCarrito);
        Carrito AgregarAlCarrito(Carrito carrito);
        Carrito ActualizarCarrito(Carrito carrito);

        bool VaciarCarrito(int idUsuario);
    }
}
