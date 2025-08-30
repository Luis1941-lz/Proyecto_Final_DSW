using Microsoft.Data.SqlClient;
using ProyectoApi_DSW.Data.Contrato;
using ProyectoApi_DSW.Models;


namespace ProyectoApi_DSW.Data.Repository
{
    public class SuscripcionesRepository : ISuscripciones
    {
        private readonly string cadenaConexion;

        public SuscripcionesRepository(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"];
        }

        public IEnumerable<PlanSuscripcion> ObtenerPlanes()
        {
            var listado = new List<PlanSuscripcion>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("Obtener_Planes", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listado.Add(new PlanSuscripcion
                            {
                                id_plan_suscripcion = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                precio = reader.GetDecimal(2),
                                duracion_dias = reader.GetInt32(3)
                            });
                        }
                    }
                }
            }

            return listado;
        }

        public PlanSuscripcion? ObtenerPlanPorId(int id)
        {
            PlanSuscripcion? plan = null;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("Obtener_PlanPorId", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_plan", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            plan = new PlanSuscripcion
                            {
                                id_plan_suscripcion = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                precio = reader.GetDecimal(2),
                                duracion_dias = reader.GetInt32(3)
                            };
                        }
                    }
                }
            }

            return plan;
        }

        public bool CrearSuscripcion(Suscripcion suscripcion)
        {
            int filas = 0;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("Insertar_Suscripcion", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id_usuario", suscripcion.id_usuario);
                    cmd.Parameters.AddWithValue("@id_plan", suscripcion.id_plan);
                    cmd.Parameters.AddWithValue("@fecha_inicio", suscripcion.fecha_inicio);
                    cmd.Parameters.AddWithValue("@fecha_fin", suscripcion.fecha_fin);
                    cmd.Parameters.AddWithValue("@estado", suscripcion.estado ?? "Activa");

                    filas = cmd.ExecuteNonQuery();
                }
            }

            return filas > 0;
        }

        public IEnumerable<Suscripcion> ObtenerSuscripcionesPorUsuario(int idUsuario)
        {
            var listado = new List<Suscripcion>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("Obtener_Suscripciones_PorUsuario", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listado.Add(new Suscripcion
                            {
                                id_suscripcion = reader.GetInt32(0),
                                id_usuario = reader.GetInt32(1),
                                id_plan = reader.GetInt32(2),
                                fecha_inicio = reader.GetDateTime(3),
                                fecha_fin = reader.GetDateTime(4),
                                estado = reader.GetString(5)
                            });
                        }
                    }
                }
            }

            return listado;
        }

        public bool CambiarTipoUsuario(int idUsuario, int idNuevoTipo)
        {
            int filas = 0;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("Cambiar_Tipo_Usuario", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.Parameters.AddWithValue("@id_tipo_usuario", idNuevoTipo);

                    filas = cmd.ExecuteNonQuery();
                }
            }

            return filas > 0;
        }


    }
}
