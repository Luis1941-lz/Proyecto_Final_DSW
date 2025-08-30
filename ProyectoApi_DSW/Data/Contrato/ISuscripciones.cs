using ProyectoApi_DSW.Models;

namespace ProyectoApi_DSW.Data.Contrato
{
    public interface ISuscripciones
    {
        IEnumerable<PlanSuscripcion> ObtenerPlanes();
        PlanSuscripcion? ObtenerPlanPorId(int id);
        bool CrearSuscripcion(Suscripcion suscripcion);
        IEnumerable<Suscripcion> ObtenerSuscripcionesPorUsuario(int idUsuario);
        bool CambiarTipoUsuario(int idUsuario, int idNuevoTipo);
    }
}
