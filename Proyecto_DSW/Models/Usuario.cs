using System.Text.Json.Serialization;

namespace Proyecto_DSW.Models
{
    public class Usuario
    {
        public int id_usuario { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string correo { get; set; }
        public string contrasena { get; set; }
        public string ? imagen { get; set; }
        public int id_tipo_usuario { get; set; }
        public int id_estado { get; set; }
        [JsonIgnore]
        public TipoUsuario? TipoUsuario { get; set; }

        [JsonIgnore]
        public Estado? Estado { get; set; }

    }
}
