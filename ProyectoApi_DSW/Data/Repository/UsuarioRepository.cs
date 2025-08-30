using ProyectoApi_DSW.Models;
using ProyectoApi_DSW.Data.Contrato;
using Microsoft.Data.SqlClient;

namespace ProyectoApi_DSW.Data.Repository
{
    public class UsuarioRepository : IUsuarios
    {
        private readonly string cadenaConexion;

        public UsuarioRepository(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DB"];
        }

        public Usuario Crear(Usuario usuario)
        {
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_CrearUsuario", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@nombre", usuario.nombre);
                    cmd.Parameters.AddWithValue("@apellido", usuario.apellido);
                    cmd.Parameters.AddWithValue("@correo", usuario.correo);
                    cmd.Parameters.AddWithValue("@contrasena", usuario.contrasena);
                    cmd.Parameters.AddWithValue("@imagen", (object)usuario.imagen ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id_tipo_usuario", usuario.id_tipo_usuario);
                    cmd.Parameters.AddWithValue("@id_estado", usuario.id_estado == 0 ? 1 : usuario.id_estado); 

                    var nuevoId = cmd.ExecuteScalar();
                    if (nuevoId != null)
                    {
                        usuario.id_usuario = Convert.ToInt32(nuevoId);
                        return usuario;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public Usuario Actualizar(Usuario usuario)
        {
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_ActualizarUsuario", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id_usuario", usuario.id_usuario);
                    cmd.Parameters.AddWithValue("@nombre", usuario.nombre);
                    cmd.Parameters.AddWithValue("@apellido", usuario.apellido);
                    cmd.Parameters.AddWithValue("@correo", usuario.correo);
                    cmd.Parameters.AddWithValue("@contrasena", usuario.contrasena);
                    cmd.Parameters.AddWithValue("@imagen", string.IsNullOrEmpty(usuario.imagen) ? DBNull.Value : (object)usuario.imagen);
                    cmd.Parameters.AddWithValue("@id_tipo_usuario", usuario.id_tipo_usuario);
                    cmd.Parameters.AddWithValue("@id_estado", usuario.id_estado);

                    int filasAfectadas = cmd.ExecuteNonQuery();
                    if (filasAfectadas > 0)
                    {
                        return usuario;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public bool Eliminar(int id)
        {
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_EliminarUsuario", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", id);

                    int filasAfectadas = cmd.ExecuteNonQuery();
                    return filasAfectadas > 0;
                }
            }
        }


        public List<Usuario> listado()
        {
            var listado = new List<Usuario>();

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand(@"
            SELECT 
                u.id_usuario, u.nombre, u.apellido, u.correo, u.contrasena, 
                u.id_tipo_usuario, tu.nombre AS tipo_usuario_nombre,
                u.id_estado, eu.nombre AS estado_nombre
            FROM Usuario u 
            INNER JOIN TipoUsuario tu ON u.id_tipo_usuario = tu.id_tipo_usuario
            INNER JOIN EstadoUsuario eu ON u.id_estado = eu.id_estado;
        ", conexion))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listado.Add(new Usuario()
                            {
                                id_usuario = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                apellido = reader.GetString(2), 
                                correo = reader.GetString(3),
                                contrasena = reader.GetString(4),
                                id_tipo_usuario = reader.GetInt32(5),
                                TipoUsuario = new TipoUsuario()
                                {
                                    id_tipo_usuario = reader.GetInt32(5),
                                    nombre = reader.GetString(6)
                                },
                                id_estado = reader.GetInt32(7),
                                Estado = new Estado()
                                {
                                    id_estado = reader.GetInt32(7),
                                    nombre = reader.GetString(8)
                                }
                            });
                        }
                    }
                }
            }

            return listado;
        }

        public Usuario ObtenerPorCorreoypassword(string correo, string contraseña)
        {
            Usuario usuario = null;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand(@"
            SELECT 
                u.id_usuario, u.nombre, u.apellido, u.correo, u.contrasena, 
                u.id_tipo_usuario, tu.nombre AS tipo_usuario_nombre,
                u.id_estado, eu.nombre AS estado_nombre
            FROM Usuario u 
            INNER JOIN TipoUsuario tu ON u.id_tipo_usuario = tu.id_tipo_usuario 
            INNER JOIN EstadoUsuario eu ON u.id_estado = eu.id_estado
            WHERE u.correo = @correo AND u.contrasena = @contrasena;", conexion))
                {
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@contrasena", contraseña);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario()
                            {
                                id_usuario = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                apellido = reader.GetString(2), 
                                correo = reader.GetString(3),
                                contrasena = reader.GetString(4),
                                id_tipo_usuario = reader.GetInt32(5),
                                TipoUsuario = new TipoUsuario()
                                {
                                    id_tipo_usuario = reader.GetInt32(5),
                                    nombre = reader.GetString(6)
                                },
                                id_estado = reader.GetInt32(7),
                                Estado = new Estado()
                                {
                                    id_estado = reader.GetInt32(7),
                                    nombre = reader.GetString(8)
                                }
                            };
                        }
                    }
                }
            }

            return usuario;
        }

        public Usuario ObtenerPorId(int id)
        {
            Usuario usuario = null;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_ObtenerUsuarioPorId", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario()
                            {
                                id_usuario = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                apellido = reader.GetString(2),
                                correo = reader.GetString(3),
                                contrasena = reader.GetString(4),
                                imagen = reader.IsDBNull(5) ? null : reader.GetString(5),
                                id_tipo_usuario = reader.GetInt32(6),
                                TipoUsuario = new TipoUsuario()
                                {
                                    id_tipo_usuario = reader.GetInt32(6),
                                    nombre = reader.GetString(7)
                                },
                                id_estado = reader.GetInt32(8),
                                Estado = new Estado()
                                {
                                    id_estado = reader.GetInt32(8),
                                    nombre = reader.GetString(9)
                                }
                            };
                        }
                    }
                }
            }

            return usuario;
        }

        public bool QuitarImagen(int id)
        {
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("sp_QuitarImagenUsuario", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", id);

                    int filasAfectadas = cmd.ExecuteNonQuery();
                    return filasAfectadas > 0;
                }
            }
        }


        public void ExpirarSuscripciones()
        {
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("EXEC ExpirarSuscripciones;", conexion))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
