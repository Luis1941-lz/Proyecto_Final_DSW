using Microsoft.Data.SqlClient;
using ProyectoApi_DSW.Data.Contrato;
using ProyectoApi_DSW.Models;

namespace ProyectoApi_DSW.Data.Repository
{
    public class TipoUsuarioRepository : ITipoUsuario
    {

        private readonly string cadenaConexion;

        public TipoUsuarioRepository(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"];
        }

        public List<TipoUsuario> listado()
        {
            var listado = new List<TipoUsuario>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("SELECT * FROM TipoUsuario", conexion))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tipoUsuario = new TipoUsuario
                            {
                                id_tipo_usuario = reader.GetInt32(0),
                                nombre = reader.GetString(1)
                            };
                            listado.Add(tipoUsuario);
                        }
                    }
                }
            }

            return listado;
        }
    }
}
