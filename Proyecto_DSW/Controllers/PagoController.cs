using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Proyecto_DSW.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http;

public class PagoController : Controller
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public PagoController(IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }


    private Suscripcion ConvertirAPlanSuscripcion(PlanSuscripciones planSuscripcion)
    {
        if (planSuscripcion.duracion_dias <= 0)
            throw new Exception("La duración del plan debe ser mayor que cero.");

        var fechaInicio = DateTime.Now;
        var fechaFin = fechaInicio.AddDays(planSuscripcion.duracion_dias);

        return new Suscripcion
        {
            id_plan = planSuscripcion.id_plan_suscripcion,
            fecha_inicio = fechaInicio,
            fecha_fin = fechaFin,
            estado = "Activa",
            Plan = planSuscripcion
        };
    }


    private async Task<List<MetodoPago>> ObtenerMetodosDePago()
    {
        using var metodoPagoHttp = new HttpClient();
        metodoPagoHttp.BaseAddress = new Uri(_config["Services:URL_API"]);

        var mensaje = await metodoPagoHttp.GetAsync("Metodopago");
        if (!mensaje.IsSuccessStatusCode)
            throw new Exception($"Error al obtener métodos de pago: {mensaje.StatusCode}");

        var data = await mensaje.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<MetodoPago>>(data);
    }

    [HttpGet]
    public async Task<IActionResult> FormularioPago(int id_plan_suscripcion, decimal precio)
    {
        var plan = new PlanSuscripciones
        {
            id_plan_suscripcion = id_plan_suscripcion,
            precio = precio,
            duracion_dias = id_plan_suscripcion switch
            {
                1 => 7,
                2 => 30,
                3 => 365,
                _ => 0
            }
        };

        if (plan.duracion_dias == 0)
        {
            ModelState.AddModelError("", "Plan no válido.");
            return View();
        }

        var suscripcion = ConvertirAPlanSuscripcion(plan);
        var pago = new Pago { Suscripcion = suscripcion };

        ViewBag.Precio = precio;
        ViewBag.IdPlanSuscripcion = id_plan_suscripcion;
        ViewBag.DuracionDias = plan.duracion_dias;
        ViewBag.MetodosDePago = new SelectList(await ObtenerMetodosDePago(), "id_metodo_pago", "nombre");

        return View(pago);
    }

    [HttpPost]
    public async Task<IActionResult> RealizarPago(Pago pago)
    {
        try
        {
            int? id_usuario = HttpContext.Session.GetInt32("usuario_id");
            if (id_usuario == null || id_usuario == 0)
                return RedirectToAction("Login", "Login");

            if (pago.id_metodo_pago <= 0)
            {
                ModelState.AddModelError("", "Debe seleccionar un método de pago.");
                ViewBag.MetodosDePago = new SelectList(await ObtenerMetodosDePago(), "id_metodo_pago", "nombre");
                return View("FormularioPago", pago);
            }

            if (pago.Suscripcion?.Plan == null)
            {
                ModelState.AddModelError("", "Plan de suscripción no válido.");
                ViewBag.MetodosDePago = new SelectList(await ObtenerMetodosDePago(), "id_metodo_pago", "nombre");
                return View("FormularioPago", pago);
            }

            pago.id_usuario = id_usuario.Value;

            pago.Suscripcion = ConvertirAPlanSuscripcion(pago.Suscripcion.Plan);
            if (pago.Suscripcion.fecha_inicio < new DateTime(1753, 1, 1))
                pago.Suscripcion.fecha_inicio = DateTime.Now;

            if (pago.Suscripcion.fecha_fin < new DateTime(1753, 1, 1))
                pago.Suscripcion.fecha_fin = pago.Suscripcion.fecha_inicio.AddDays(pago.Suscripcion.Plan.duracion_dias);

            switch (pago.id_metodo_pago)
            {
                case 1: 
                    pago.monto = pago.Suscripcion.Plan.precio * 1.05M; 
                    break;
                case 2: 
                    pago.monto = pago.Suscripcion.Plan.precio;
                    break;
                case 3: 
                    pago.monto = pago.Suscripcion.Plan.precio * 0.98M; 
                    break;
                case 4: 
                    pago.monto = pago.Suscripcion.Plan.precio * 1.03M;
                    break;
                case 5: 
                    pago.monto = pago.Suscripcion.Plan.precio;
                    break;
                default:
                    pago.monto = pago.Suscripcion.Plan.precio;
                    break;
            }


            var requestBody = new
            {
                id_usuario = pago.id_usuario,
                id_metodo_pago = pago.id_metodo_pago,
                monto = pago.monto,
                estado = "Exitoso",
                suscripcion = new
                {
                    id_plan = pago.Suscripcion.id_plan,
                    fecha_inicio = pago.Suscripcion.fecha_inicio.ToString("yyyy-MM-ddTHH:mm:ss.fff")
                }
            };

            var contenido = new StringContent(
                JsonConvert.SerializeObject(requestBody),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync($"{_config["Services:URL_API"]}Pago/realizar-pago", contenido);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al procesar el pago: {response.StatusCode} - {errorContent}");
            }

            var data = await response.Content.ReadAsStringAsync();
            var resultadoPago = JsonConvert.DeserializeObject<Pago>(data);

            if (resultadoPago != null && resultadoPago.estado == "Exitoso")
            {
                HttpContext.Session.SetInt32("usuario_tipo", 1);
                HttpContext.Session.SetInt32("usuario_estado", 1);

                return RedirectToAction("ConfirmacionPago");
            }

            ModelState.AddModelError("", $"El pago no fue exitoso. Estado real: {resultadoPago?.estado ?? "Desconocido"}");
            ViewBag.MetodosDePago = new SelectList(await ObtenerMetodosDePago(), "id_metodo_pago", "nombre");
            return View("FormularioPago", pago);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al realizar el pago: {ex.Message}");
            ViewBag.MetodosDePago = new SelectList(await ObtenerMetodosDePago(), "id_metodo_pago", "nombre");
            return View("FormularioPago", pago);
        }
    }


    public IActionResult ConfirmacionPago()
    {
        return View();
    }
}
