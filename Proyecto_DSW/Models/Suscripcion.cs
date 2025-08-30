namespace Proyecto_DSW.Models
{
    public class Suscripcion
    {
        public int id_suscripcion { get; set; }
        public int id_usuario { get; set; }  // ID del usuario asociado
        public int id_plan { get; set; }  // ID del plan de suscripción
        public DateTime fecha_inicio { get; set; }
        public DateTime fecha_fin { get; set; }
        public string estado { get; set; }  // Ejemplo: "Activa", "Cancelada"

        // Relación con Usuario
        public Usuario Usuario { get; set; }

        // Relación con Plan
        public PlanSuscripciones Plan { get; set; }
    }
}
