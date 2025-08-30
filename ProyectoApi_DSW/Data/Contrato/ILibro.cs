using ProyectoApi_DSW.Models;

namespace ProyectoApi_DSW.Data.Contrato
{
    public interface ILibro
    {
        List<Libro> ObtenerLibro();
        Libro ObtenerPorId(int id);
        Libro Crear(Libro libro);
        Libro Actualizar(Libro libro);
        Boolean Eliminar(int id);
    }
}
