using Microsoft.Data.SqlClient;
using ProyectoApi_DSW.Data.Contrato;
using ProyectoApi_DSW.Models;

namespace ProyectoApi_DSW.Data.Repository
{
    public class EstadoUsuarioRepository : IEstadoUsuario
    {

        private readonly string cadenaConexion;

        public EstadoUsuarioRepository(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"];
        }

        public List<Estado> listado()
        {
            var listado = new List<Estado>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("SELECT * FROM EstadoUsuario", conexion))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var estado = new Estado
                            {
                                id_estado = reader.GetInt32(0),
                                nombre = reader.GetString(1)
                            };
                            listado.Add(estado);
                        }
                    }
                }
            }

            return listado;
        }
        

        
    }
}
