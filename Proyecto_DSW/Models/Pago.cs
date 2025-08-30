namespace Proyecto_DSW.Models
{
    public class Pago
    {
        internal int? id_suscripcion;  // Campo interno que posiblemente no se necesite en este contexto, ya que Suscripcion está ya como propiedad

        public int id_pago { get; set; }  // Identificador único del pago

        public int id_usuario { get; set; }  // Referencia al usuario que realiza el pago

        public int id_metodo_pago { get; set; }  // Referencia al método de pago

        public decimal monto { get; set; }  // Monto pagado

        public DateTime fecha_pago { get; set; } = DateTime.Now;  // Fecha y hora del pago

        public string estado { get; set; }  // Estado del pago ("Exitoso", "Fallido", "Pendiente")

        // Relación con el usuario
        public Usuario Usuario { get; set; }

        // Relación con el método de pago
        public MetodoPago MetodoPago { get; set; }

        // Relación con la suscripción (solo si el pago está asociado a una suscripción)
        public Suscripcion Suscripcion { get; set; }
    }
}
