
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Peactica_API.Models;

#region Configuracion 
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<Practica1Context>(op =>
    op.UseSqlServer(builder.Configuration.GetConnectionString("Practica1")));


//prueba para la conexion de la bd
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#endregion
//Pruba la conexion con la base de datos
app.MapGet("/Conectar", async (IConfiguration configuration) =>
{
    string? connectionString = configuration.GetConnectionString("Practica1");

    if (string.IsNullOrEmpty(connectionString))
    {
        return Results.Problem("Error: Connection string is null or empty", statusCode: 500);
    }

    try
    {
        await using var conexion = new SqlConnection(connectionString);
        await conexion.OpenAsync();
        return Results.Ok("Conexión Exitosa");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error: {ex.Message}", statusCode: 500);
    }
})
.WithOpenApi();

//Extrae la lista de los restaurantes
app.MapGet("/api/Restaurantes", async (Practica1Context context) =>
{
    var restaurantes = await context.Restaurantes.ToListAsync();
    if (restaurantes.Count > 0)
        return Results.Ok(restaurantes);
    return Results.NoContent();
});

//Agrega un nuevo restaurante
app.MapPost("/api/AgregarRestaurante", async (Restaurantes restaurante, Practica1Context context) =>
{
    if (
        string.IsNullOrEmpty(restaurante.Nombre)    ||
        string.IsNullOrEmpty(restaurante.Dueño)     ||
        string.IsNullOrEmpty(restaurante.Provincia) ||
        string.IsNullOrEmpty(restaurante.Canton)    ||
        string.IsNullOrEmpty(restaurante.Distrito))
    {
        return Results.BadRequest("Todos los datos son requeridos");
    }
    context.Add(restaurante);
    await context.SaveChangesAsync();
    return Results.Created($"/api/Restaurantes/{restaurante.Id}", restaurante);
});

//Busca un restaurante por Id

app.MapGet("/api/Restaurantes/{id}", async (int id, Practica1Context context) =>
{
    var restaurante = await context.Restaurantes.FindAsync(id);
    if (restaurante == null)
        return Results.NotFound("Restaurante no encontrado");
    return Results.Ok(restaurante);
});

//Modifica un restaurante  por Id si las entradas son nulas las deja igual 
app.MapPut("/api/ActualizarRestaurante", async ([FromQuery] int id, Restaurantes restaurante, Practica1Context context) =>
{
    var dbRestaurante = await context.Restaurantes.FindAsync(id);

    if (dbRestaurante == null)
        return Results.NotFound("Restaurante no encontrado");

    // Actualizar solo los campos proporcionados
    if (!string.IsNullOrEmpty(restaurante.Nombre))
        dbRestaurante.Nombre = restaurante.Nombre;

    if (!string.IsNullOrEmpty(restaurante.DireccionExacta))
        dbRestaurante.DireccionExacta = restaurante.DireccionExacta;

    if (!string.IsNullOrEmpty(restaurante.Canton))
        dbRestaurante.Canton = restaurante.Canton;

    if (!string.IsNullOrEmpty(restaurante.Provincia))
        dbRestaurante.Provincia = restaurante.Provincia;

    if (!string.IsNullOrEmpty(restaurante.Distrito))
        dbRestaurante.Distrito = restaurante.Distrito;

    await context.SaveChangesAsync();
    return Results.Ok(dbRestaurante);
});



//Elimina un restaurante por Id
app.MapDelete("/api/EliminarRestaurante/{id:int:min(1):max(100)}", async (int id, Practica1Context context) =>
{
    var restaurante = await context.Restaurantes.FindAsync(id);
    if (restaurante == null)
        return Results.NotFound("Restaurante no encontrado");
    context.Remove(restaurante);
    await context.SaveChangesAsync();
    return Results.Ok("Restaurante eliminado");
});


#region Endpoints

#endregion

app.Run();




