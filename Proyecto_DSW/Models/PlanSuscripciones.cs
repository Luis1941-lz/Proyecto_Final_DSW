namespace Proyecto_DSW.Models
{
    public class PlanSuscripciones
    {

        public int id_plan_suscripcion { get; set; }
        public string nombre { get; set; }
        public decimal precio { get; set; }
        public int duracion_dias { get; set; }
    }
}
