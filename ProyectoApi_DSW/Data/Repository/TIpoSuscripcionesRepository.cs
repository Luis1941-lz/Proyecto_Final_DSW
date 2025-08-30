using Microsoft.Data.SqlClient;
using ProyectoApi_DSW.Data.Contrato;
using ProyectoApi_DSW.Models;


namespace ProyectoApi_DSW.Data.Repository
{
    public class TIpoSuscripcionesRepository : ITipoSuscripciones
    {
        private readonly string cadenaConexion;

        public TIpoSuscripcionesRepository(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"];
        }

        public List<PlanSuscripcion> ObtenerTipoSuscripciones()
        {
            var listado = new List<PlanSuscripcion>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                using (var cmd = new SqlCommand("Select * From PlanSuscripcion", conexion))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listado.Add(new PlanSuscripcion()
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
    }
}
