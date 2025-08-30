namespace ProyectoApi_DSW.Models.DTO
{
    public class PagoRequest
    {
        public int id_usuario { get; set; }
        public int id_metodo_pago { get; set; }
        public decimal monto { get; set; }
        public string estado { get; set; }
        public SuscripcionRequest Suscripcion { get; set; }
    }

    public class SuscripcionRequest
    {
        public int id_plan { get; set; }
        public DateTime fecha_inicio { get; set; }
    }

}
