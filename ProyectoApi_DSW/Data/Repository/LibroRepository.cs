using ProyectoApi_DSW.Models;
using ProyectoApi_DSW.Data.Contrato;
using Microsoft.Data.SqlClient;


namespace ProyectoApi_DSW.Data.Repository
{
    public class LibroRepository : ILibro
    {
        private readonly string cadenaConexion;

        public LibroRepository(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"];
        }

        public Libro Actualizar(Libro libro)
        {
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("Actualizar_Libro", conexion))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_libro", libro.id_libro);
                    command.Parameters.AddWithValue("@titulo", libro.titulo);
                    command.Parameters.AddWithValue("@autor", libro.autor ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@precio", libro.precio);
                    command.Parameters.AddWithValue("@stock", libro.stock);
                    command.Parameters.AddWithValue("@descripcion", libro.descripcion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@tiene_pdf", libro.tiene_pdf);
                    command.Parameters.AddWithValue("@pdf_url", string.IsNullOrEmpty(libro.pdf_url) ? (object)DBNull.Value : libro.pdf_url);
                    command.Parameters.AddWithValue("@imagen", string.IsNullOrEmpty(libro.imagen) ? (object)DBNull.Value : libro.imagen);
                    command.Parameters.AddWithValue("@id_categoria", libro.id_categoria);

                    command.ExecuteNonQuery();
                }
            }
            return ObtenerPorId(libro.id_libro);
        }

        public Libro Crear(Libro libro)
        {
            try
            {
                Libro nuevoLibro = null;
                int nuevoID = 0;

                using (var conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();
                    using (var command = new SqlCommand("Insertar_Libro", conexion))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@titulo", libro.titulo ?? "");
                        command.Parameters.AddWithValue("@autor", libro.autor ?? "");
                        command.Parameters.AddWithValue("@precio", libro.precio);
                        command.Parameters.AddWithValue("@stock", libro.stock);
                        command.Parameters.AddWithValue("@descripcion", libro.descripcion ?? "");
                        command.Parameters.AddWithValue("@tiene_pdf", libro.tiene_pdf);
                        command.Parameters.AddWithValue("@pdf_url", libro.pdf_url ?? "");
                        command.Parameters.AddWithValue("@imagen", libro.imagen ?? "");
                        command.Parameters.AddWithValue("@id_categoria", libro.id_categoria);

                        var result = command.ExecuteScalar();
                        if (result == null)
                            throw new Exception("No se pudo insertar el libro en la BD.");

                        nuevoID = Convert.ToInt32(result);
                    }
                }

                nuevoLibro = ObtenerPorId(nuevoID);
                return nuevoLibro;
            }
            catch (Exception ex)
            {
                throw new Exception("Error en LibroRepository.Crear: " + ex.Message);
            }
        }


        public bool Eliminar(int id)
        {
            int filas = 0;
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("Eliminar_Libro", conexion))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_libro", id);
                    filas = command.ExecuteNonQuery();
                }
            }
            return filas > 0;
        }

        public List<Libro> ObtenerLibro()
        {
            var listado = new List<Libro>();
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand(@"SELECT l.id_libro, l.titulo, l.autor, l.precio, l.stock, l.descripcion, l.tiene_pdf, l.pdf_url, l.imagen, l.id_categoria, 
                                                    c.id_categoria, c.nombre FROM Libro l INNER JOIN CategoriaLibro c ON l.id_categoria = c.id_categoria;", conexion))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listado.Add(new Libro()
                            {
                                id_libro = reader.GetInt32(0),
                                titulo = reader.GetString(1),
                                autor = reader.GetString(2),
                                precio = reader.GetDecimal(3),
                                stock = reader.GetInt32(4),
                                descripcion = reader.GetString(5),
                                tiene_pdf = reader.GetBoolean(6),
                                pdf_url = reader.IsDBNull(7) ? null : reader.GetString(7),
                                imagen = reader.IsDBNull(8) ? null : reader.GetString(8),
                                id_categoria = reader.GetInt32(9),
                                Categoria = new CategoriaLibro()
                                {
                                    id_categoria = reader.GetInt32(10),
                                    nombre = reader.GetString(11)
                                }

                            });
                        }
                    }
                }
            }
            return listado;
        }

        public Libro ObtenerPorId(int id)
        {
            Libro libro = null;
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("Obtener_LibroPorId", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_libro", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            libro = new Libro()
                            {
                                id_libro = reader.GetInt32(0),
                                titulo = reader.GetString(1),
                                autor = reader.IsDBNull(2) ? null : reader.GetString(2),
                                precio = reader.GetDecimal(3),
                                stock = reader.GetInt32(4),
                                descripcion = reader.IsDBNull(5) ? null : reader.GetString(5),
                                tiene_pdf = reader.GetBoolean(6),
                                pdf_url = reader.IsDBNull(7) ? null : reader.GetString(7),
                                imagen = reader.IsDBNull(8) ? null : reader.GetString(8),
                                id_categoria = reader.GetInt32(9),
                                Categoria = new CategoriaLibro()
                                {
                                    id_categoria = reader.GetInt32(9),  
                                    nombre = reader.GetString(10)       
                                }
                            };
                        }
                    }
                }
            }
            return libro;
        }
    }
}
