using Microsoft.Data.SqlClient;
using ProyectoApi_DSW.Data.Contrato;
using ProyectoApi_DSW.Models;
using System.Data;

namespace ProyectoApi_DSW.Data.Repository
{
    public class PagoRepository : IPago
    {
        private readonly string cadenaConexion;

        public PagoRepository(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"];
        }

        public List<Pago> ObtenerPagosPorUsuario(int id_usuario)
        {
            var pagos = new List<Pago>();
            using var conexion = new SqlConnection(cadenaConexion);
            conexion.Open();

            using var cmd = new SqlCommand("SELECT * FROM Pago WHERE id_usuario = @id_usuario", conexion);
            cmd.Parameters.AddWithValue("@id_usuario", id_usuario);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                pagos.Add(new Pago
                {
                    id_pago = reader.GetInt32(0),
                    id_usuario = reader.GetInt32(1),
                    id_metodo_pago = reader.GetInt32(2),
                    monto = reader.GetDecimal(3),
                    fecha_pago = reader.GetDateTime(4),
                    estado = reader.GetString(5),
                    id_suscripcion = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6)
                });
            }

            return pagos;
        }

        public Pago ObtenerUltimoPago(int id_usuario)
        {
            Pago pago = null;
            using var conexion = new SqlConnection(cadenaConexion);
            conexion.Open();

            using var cmd = new SqlCommand(
                "SELECT TOP 1 * FROM Pago WHERE id_usuario = @id_usuario ORDER BY fecha_pago DESC",
                conexion
            );
            cmd.Parameters.AddWithValue("@id_usuario", id_usuario);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                pago = new Pago
                {
                    id_pago = reader.GetInt32(0),
                    id_usuario = reader.GetInt32(1),
                    id_metodo_pago = reader.GetInt32(2),
                    monto = reader.GetDecimal(3),
                    fecha_pago = reader.GetDateTime(4),
                    estado = reader.GetString(5),
                    id_suscripcion = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6)
                };
            }

            return pago;
        }

        public Pago RealizarPago(Pago pago)
        {
            try
            {
                using var conexion = new SqlConnection(cadenaConexion);
                conexion.Open();

                using var command = new SqlCommand("RealizarPago", conexion);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 60;


                var fechaInicio = pago.Suscripcion?.fecha_inicio ?? DateTime.Now;
                if (fechaInicio < new DateTime(1753, 1, 1)) fechaInicio = DateTime.Now;

                var fechaFin = pago.Suscripcion?.fecha_fin ?? fechaInicio.AddDays(pago.Suscripcion?.Plan?.duracion_dias ?? 30);
                if (fechaFin < new DateTime(1753, 1, 1)) fechaFin = fechaInicio.AddDays(pago.Suscripcion?.Plan?.duracion_dias ?? 30);

                // Parámetros
                command.Parameters.AddWithValue("@id_usuario", pago.id_usuario);
                command.Parameters.AddWithValue("@id_plan", pago.Suscripcion?.id_plan ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@id_metodo_pago", pago.id_metodo_pago);
                command.Parameters.AddWithValue("@monto", pago.monto);
                command.Parameters.AddWithValue("@estado", pago.estado ?? "Pendiente");
                command.Parameters.AddWithValue("@fecha_inicio", fechaInicio);
                command.Parameters.AddWithValue("@fecha_fin", fechaFin);

                command.ExecuteNonQuery();

                return ObtenerUltimoPago(pago.id_usuario);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en PagoRepository.RealizarPago: " + ex.Message, ex);
            }
        }
    }
}
