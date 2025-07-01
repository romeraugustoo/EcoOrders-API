using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrdenesAPI.Data;
using OrdenesAPI.Services;
using OrdenesAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 🔌 Configurar EF Core con SQL Server Express
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Agregar servicios necesarios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "EcoOrders API",
            Version = "v1",
            Description =
                @"TP 01 - Desarrollo de un Servicio en C# con API - Módulo de Órdenes

Integrantes del Grupo:
- Romera Rodríguez, August Efrain (Legajo: 48446)
- Romano, Emilise Milena (Legajo: 57249)
- Romano, Luis Fernando (Legajo: 57248)
- Correa, Horacio David  (Legajo: 52314)

Materia: Desarrollo de Software - Universidad Tecnológica Nacional – Facultad Regional Tucumán

Nombre del Proyecto: TP 01 - Módulo de Órdenes (EcoOrders API)",
        }
    );

    // Habilitar comentarios XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Configuración de servicios personalizados
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<OrderService>();

// Configuración JSON para evitar ciclos
builder
    .Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var app = builder.Build();

// 🧪 Activar Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EcoOrders API v1");
        c.RoutePrefix = "swagger"; // Opcional: define la ruta de acceso
        c.DocumentTitle = "EcoOrders API Documentation";
    });
}

// Middleware HTTP
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// 🌱 Inicialización de datos semilla
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        // Solo inicializar datos si estamos en desarrollo o si la tabla está vacía
        if (app.Environment.IsDevelopment() || !context.Products.Any())
        {
            SeedData.Initialize(context);
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al inicializar los datos semilla");
    }
}

app.Run();
