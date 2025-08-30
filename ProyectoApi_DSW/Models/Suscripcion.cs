
namespace ProyectoApi_DSW.Models
{
    public class Suscripcion
    {
        public int id_suscripcion { get; set; }

        public int id_usuario { get; set; }

        public int id_plan { get; set; }

        public DateTime fecha_inicio { get; set; }

        public DateTime fecha_fin { get; set; }

        public string estado { get; set; } = "Activa";


        public Usuario Usuario { get; set; }

        public PlanSuscripcion Plan { get; set; }
    }
}
