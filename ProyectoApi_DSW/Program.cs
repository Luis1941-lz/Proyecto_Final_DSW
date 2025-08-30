using ProyectoApi_DSW.Data.Contrato;
using ProyectoApi_DSW.Data.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUsuarios, UsuarioRepository>();
builder.Services.AddScoped<ILibro, LibroRepository>();
builder.Services.AddScoped<ITipoSuscripciones, TIpoSuscripcionesRepository>();
builder.Services.AddScoped<ICategoriaLibro, CategoriaLibroRepository>();
builder.Services.AddScoped<ISuscripciones, SuscripcionesRepository>();
builder.Services.AddScoped<IMetodoPago, MetodoPagoRepository>();
builder.Services.AddScoped<IPago, PagoRepository>();
builder.Services.AddScoped<ICarrito, CarritoRepository>();
builder.Services.AddScoped<IVenta, VentaRepository>();
builder.Services.AddScoped<IDetalleVenta, DetalleVentaRepository>();
builder.Services.AddScoped<ITipoUsuario, TipoUsuarioRepository>();
builder.Services.AddScoped<IEstadoUsuario, EstadoUsuarioRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
