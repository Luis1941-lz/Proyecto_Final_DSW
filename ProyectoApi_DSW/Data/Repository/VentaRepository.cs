using Microsoft.Data.SqlClient;
using ProyectoApi_DSW.Data.Contrato;
using ProyectoApi_DSW.Models;
using System.Data;

namespace ProyectoApi_DSW.Data.Repository
{
    public class VentaRepository : IVenta
    {
        private readonly string _cadenaConexion;

        public VentaRepository(IConfiguration config)
        {
            _cadenaConexion = config["ConnectionStrings:DB"];
        }

        public Venta CrearVenta(Venta venta)
        {
            try
            {
                int nuevoID = 0;
                using (var conexion = new SqlConnection(_cadenaConexion))
                {
                    conexion.Open();
                    using (var command = new SqlCommand("Insertar_Venta", conexion))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_usuario", venta.id_usuario);
                        command.Parameters.AddWithValue("@id_metodo_pago", venta.id_metodo_pago);
                        command.Parameters.AddWithValue("@fecha_venta", venta.fecha_venta);

                        var result = command.ExecuteScalar();
                        if (result == null)
                            throw new Exception("No se pudo insertar la venta.");

                        nuevoID = Convert.ToInt32(result);
                    }
                }

                return ObtenerVentaPorId(nuevoID);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en VentaRepository.CrearVenta: " + ex.Message);
            }
        }

        public List<Venta> ObtenerVentasPorUsuario(int idUsuario)
        {
            var ventas = new List<Venta>();

            using (SqlConnection cn = new SqlConnection(_cadenaConexion))
            {
                SqlCommand cmd = new SqlCommand("Obtener_VentasPorUsuario", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                var ventasDict = new Dictionary<int, Venta>();

                while (dr.Read())
                {
                    int idVenta = Convert.ToInt32(dr["id_venta"]);

                    if (!ventasDict.ContainsKey(idVenta))
                    {
                        var venta = new Venta
                        {
                            id_venta = idVenta,
                            fecha_venta = Convert.ToDateTime(dr["fecha_venta"]),
                            id_usuario = Convert.ToInt32(dr["id_usuario"]),
                            Usuario = new Usuario
                            {
                                id_usuario = Convert.ToInt32(dr["id_usuario"]),
                                nombre = dr["usuario_nombre"].ToString(),
                                apellido = dr["usuario_apellido"].ToString(),
                                correo = dr["usuario_correo"].ToString()
                            },
                            id_metodo_pago = Convert.ToInt32(dr["id_metodo_pago"]),
                            MetodoPago = new MetodoPago
                            {
                                id_metodo_pago = Convert.ToInt32(dr["id_metodo_pago"]),
                                nombre = dr["metodo_pago_nombre"].ToString()
                            },
                            DetallesVenta = new List<DetalleVenta>()
                        };

                        ventasDict.Add(idVenta, venta);
                    }

                    var detalle = new DetalleVenta
                    {
                        id_detalle = Convert.ToInt32(dr["id_detalle"]),
                        id_venta = idVenta,
                        id_libro = Convert.ToInt32(dr["id_libro"]),
                        cantidad = Convert.ToInt32(dr["cantidad"]),
                        Libro = new Libro
                        {
                            id_libro = Convert.ToInt32(dr["id_libro"]),
                            titulo = dr["libro_titulo"].ToString(),
                            autor = dr["libro_autor"].ToString(),
                            precio = Convert.ToDecimal(dr["libro_precio"])
                        }
                    };

                    ventasDict[idVenta].DetallesVenta.Add(detalle);
                }

                ventas = ventasDict.Values.ToList();
            }

            return ventas;
        }



        public Venta ObtenerVentaPorId(int idVenta)
        {
            Venta venta = null;
            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("Obtener_VentaPorId", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_venta", idVenta);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            venta = new Venta()
                            {
                                id_venta = reader.GetInt32(reader.GetOrdinal("id_venta")),
                                fecha_venta = reader.GetDateTime(reader.GetOrdinal("fecha_venta")),

                                Usuario = new Usuario()
                                {
                                    id_usuario = reader.GetInt32(reader.GetOrdinal("UsuarioId")),
                                    nombre = reader.GetString(reader.GetOrdinal("UsuarioNombre")),
                                    apellido = reader.GetString(reader.GetOrdinal("UsuarioApellido")),
                                    correo = reader.GetString(reader.GetOrdinal("UsuarioCorreo"))
                                },

                                MetodoPago = new MetodoPago()
                                {
                                    id_metodo_pago = reader.GetInt32(reader.GetOrdinal("MetodoPagoId")),
                                    nombre = reader.GetString(reader.GetOrdinal("MetodoPagoNombre"))
                                }
                            };
                        }
                    }
                }
            }
            return venta;
        }

        public List<Venta> ObtenerTodasLasVentas()
        {
            var ventas = new List<Venta>();
            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("Obtener_TodasLasVentas", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ventas.Add(new Venta()
                            {
                                id_venta = reader.GetInt32(reader.GetOrdinal("id_venta")),
                                id_usuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                                id_metodo_pago = reader.GetInt32(reader.GetOrdinal("id_metodo_pago")),
                                fecha_venta = reader.GetDateTime(reader.GetOrdinal("fecha_venta"))
                            });
                        }
                    }
                }
            }
            return ventas;
        }

        public bool EliminarVenta(int idVenta)
        {
            int filas = 0;
            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("Eliminar_Venta", conexion))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_venta", idVenta);
                    filas = command.ExecuteNonQuery();
                }
            }
            return filas > 0;
        }

        public Venta ActualizarVenta(Venta venta)
        {
            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("Actualizar_Venta", conexion))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_venta", venta.id_venta);
                    command.Parameters.AddWithValue("@id_usuario", venta.id_usuario);
                    command.Parameters.AddWithValue("@id_metodo_pago", venta.id_metodo_pago);
                    command.Parameters.AddWithValue("@fecha_venta", venta.fecha_venta);

                    command.ExecuteNonQuery();
                }
            }
            return ObtenerVentaPorId(venta.id_venta);
        }
    }
}
