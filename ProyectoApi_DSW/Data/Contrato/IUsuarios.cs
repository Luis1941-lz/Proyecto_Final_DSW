using ProyectoApi_DSW.Models;

namespace ProyectoApi_DSW.Data.Contrato
{
    public interface IUsuarios
    {
        List<Usuario> listado();
        Usuario ObtenerPorId(int id);
        Usuario ObtenerPorCorreoypassword(string correo, string contraseña);
        Usuario Crear(Usuario usuario);
        Usuario Actualizar(Usuario usuario);
        Boolean Eliminar(int id);
        bool QuitarImagen(int id);
        public void ExpirarSuscripciones();
    }
}
