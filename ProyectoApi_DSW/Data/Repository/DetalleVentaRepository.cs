using ProyectoApi_DSW.Models;
using ProyectoApi_DSW.Data.Contrato;
using Microsoft.Data.SqlClient;

namespace ProyectoApi_DSW.Data.Repository
{
    public class DetalleVentaRepository : IDetalleVenta
    {
        private readonly string _cadenaConexion;

        public DetalleVentaRepository(IConfiguration config)
        {
            _cadenaConexion = config["ConnectionStrings:DB"];
        }

        public DetalleVenta CrearDetalleVenta(DetalleVenta detalleVenta)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            conexion.Open();

            using var cmd = new SqlCommand("Insertar_DetalleVenta", conexion);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id_venta", detalleVenta.id_venta);
            cmd.Parameters.AddWithValue("@id_libro", detalleVenta.id_libro);
            cmd.Parameters.AddWithValue("@cantidad", detalleVenta.cantidad);

            var result = cmd.ExecuteScalar();
            detalleVenta.id_detalle = Convert.ToInt32(result);

            return detalleVenta;
        }

        public List<DetalleVenta> ObtenerDetallesVenta(int idVenta)
        {
            var detalles = new List<DetalleVenta>();

            using var conexion = new SqlConnection(_cadenaConexion);
            conexion.Open();

            using var cmd = new SqlCommand("Obtener_DetallesVenta", conexion);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id_venta", idVenta);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                detalles.Add(new DetalleVenta
                {
                    id_detalle = reader.GetInt32(0),
                    id_venta = reader.GetInt32(1),
                    id_libro = reader.GetInt32(2),
                    cantidad = reader.GetInt32(3),
                    Libro = new Libro
                    {
                        id_libro = reader.GetInt32(2),
                        titulo = reader.GetString(4),
                        precio = reader.GetDecimal(5)
                    }
                });
            }

            return detalles;
        }
    }
}
