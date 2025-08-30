

namespace Proyecto_DSW.Models.DTO
{
    public class ConfirmarCompraVista
    {
        public int IdUsuario { get; set; }
        public List<Carrito> Detalles { get; set; }

        public Usuario ? Usuario { get; set; }

        public Libro ? Libro { get; set; }

        public int IdMetodoPago { get; set; }

        public MetodoPago? metodoPago { get; set; }

    }
}
