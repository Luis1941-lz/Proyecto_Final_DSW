using Microsoft.Data.SqlClient;
using ProyectoApi_DSW.Data.Contrato;
using ProyectoApi_DSW.Models;

namespace ProyectoApi_DSW.Data.Repository
{
        public class CarritoRepository : ICarrito
        {
            private readonly string _cadenaConexion;

            public CarritoRepository(IConfiguration config)
            {
                _cadenaConexion = config["ConnectionStrings:DB"];
            }

            public List<Carrito> ObtenerCarritoPorUsuario(int idUsuario)
            {
                var listado = new List<Carrito>();
                using (var conexion = new SqlConnection(_cadenaConexion))
                {
                    conexion.Open();
                    using (var cmd = new SqlCommand("Obtener_CarritoPorUsuario", conexion))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_usuario", idUsuario);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listado.Add(new Carrito()
                                {
                                    IdCarrito = reader.GetInt32(0),
                                    IdUsuario = reader.GetInt32(1),
                                    IdLibro = reader.GetInt32(2),
                                    Cantidad = reader.GetInt32(3),

                                    Libro = new Libro()
                                    {
                                        id_libro = reader.GetInt32(2),
                                        titulo = reader.GetString(4),
                                        autor = reader.GetString(5),
                                        precio = reader.GetDecimal(6)
                                    }
                                });
                            }
                        }
                    }
                }
                return listado;
            }

            public Carrito ObtenerPorId(int idCarrito)
            {
                Carrito carrito = null;
                using (var conexion = new SqlConnection(_cadenaConexion))
                {
                    conexion.Open();
                    using (var cmd = new SqlCommand("Obtener_CarritoPorId", conexion))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_carrito", idCarrito);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                carrito = new Carrito()
                                {
                                    IdCarrito = reader.GetInt32(0),
                                    IdUsuario = reader.GetInt32(1),
                                    IdLibro = reader.GetInt32(2),
                                    Cantidad = reader.GetInt32(3),
                                    Libro = new Libro()
                                    {
                                        id_libro = reader.GetInt32(2),
                                        titulo = reader.GetString(4),
                                        autor = reader.GetString(5),
                                        precio = reader.GetDecimal(6)
                                    }
                                };
                            }
                        }
                    }
                }
                return carrito;
            }

            public Carrito AgregarAlCarrito(Carrito carrito)
            {
                int nuevoId = 0;

                using (var conexion = new SqlConnection(_cadenaConexion))
                {
                    conexion.Open();
                    using (var command = new SqlCommand("Insertar_Carrito", conexion))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_usuario", carrito.IdUsuario);
                        command.Parameters.AddWithValue("@id_libro", carrito.IdLibro);
                        command.Parameters.AddWithValue("@cantidad", carrito.Cantidad);


                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                nuevoId = reader.GetInt32(0);
                            }
                        }
                    }
                }

                carrito.IdCarrito = nuevoId; 
                return ObtenerPorId(nuevoId); 
            }




            public Carrito ActualizarCarrito(Carrito carrito)
            {
                using (var conexion = new SqlConnection(_cadenaConexion))
                {
                    conexion.Open();
                    using (var command = new SqlCommand("Actualizar_Carrito", conexion))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_carrito", carrito.IdCarrito);
                        command.Parameters.AddWithValue("@id_usuario", carrito.IdUsuario);
                        command.Parameters.AddWithValue("@id_libro", carrito.IdLibro);
                        command.Parameters.AddWithValue("@cantidad", carrito.Cantidad);

                        command.ExecuteNonQuery();
                    }
                }
                return ObtenerPorId(carrito.IdCarrito);
            }

            public bool VaciarCarrito(int idUsuario)
            {
                int filas = 0;
                using (var conexion = new SqlConnection(_cadenaConexion))
                {
                    conexion.Open();
                    using (var command = new SqlCommand("Vaciar_Carrito", conexion))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_usuario", idUsuario);
                        filas = command.ExecuteNonQuery();
                    }
                }
                return filas > 0;
            }
        }
    }

