using ProyectoApi_DSW.Models;

namespace ProyectoApi_DSW.Data.Contrato
{
    public interface IPago
    {
        Pago RealizarPago(Pago pago);

        Pago ObtenerUltimoPago(int id_usuario);

        List<Pago> ObtenerPagosPorUsuario(int id_usuario);
    }
}
