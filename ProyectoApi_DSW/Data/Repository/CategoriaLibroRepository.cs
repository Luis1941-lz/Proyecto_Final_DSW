using ProyectoApi_DSW.Models;
using ProyectoApi_DSW.Data.Contrato;
using Microsoft.Data.SqlClient;


namespace ProyectoApi_DSW.Data.Repository
{
    public class CategoriaLibroRepository : ICategoriaLibro
    {
        private readonly string cadenaConexion;

        public CategoriaLibroRepository(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"];
        }

        public List<CategoriaLibro> obtenerCategoria()
        {
            var listado = new List<CategoriaLibro>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("SELECT id_categoria, nombre FROM CategoriaLibro", conexion))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var categoria = new CategoriaLibro
                            {
                                id_categoria = reader.GetInt32(0),
                                nombre = reader.GetString(1)
                            };
                            listado.Add(categoria);
                        }
                    }
                }
            }

            return listado;
        }
    }
}
