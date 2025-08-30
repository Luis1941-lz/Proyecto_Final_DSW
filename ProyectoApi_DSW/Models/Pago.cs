
namespace ProyectoApi_DSW.Models;
    using System;

    public class Pago       
    {
        internal int? id_suscripcion;

        public int id_pago { get; set; }

        public int id_usuario { get; set; }  

        public int id_metodo_pago { get; set; }  

        public decimal monto { get; set; }  

        public DateTime fecha_pago { get; set; } = DateTime.Now;  

        public string estado { get; set; }  

        public Usuario Usuario { get; set; }
        public MetodoPago MetodoPago { get; set; }
        public Suscripcion Suscripcion { get; set; }
    }

