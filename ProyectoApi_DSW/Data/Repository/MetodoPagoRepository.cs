using ProyectoApi_DSW.Models;
using ProyectoApi_DSW.Data.Contrato;
using Microsoft.Data.SqlClient;

namespace ProyectoApi_DSW.Data.Repository
{
    public class MetodoPagoRepository : IMetodoPago
    {

        private readonly string cadenaConexion;

        public MetodoPagoRepository(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"];
        }

        public List<MetodoPago> listarMetodoPago()
        {
            var listado = new List<MetodoPago>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                using (var cmd = new SqlCommand("Select * From MetodoPago", conexion))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listado.Add(new MetodoPago()
                            {
                                id_metodo_pago = reader.GetInt32(0),
                                nombre = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return listado;
        }
    }
}
