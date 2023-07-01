using ConcurrenciaApi.Data;
using ConcurrenciaApi.Tablas;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddDbContext<ConcurrenciaDbContext>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("Conexion"));
});
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.MapPost("/Usuarios", async (ConcurrenciaDbContext context, ILogger<Program> logger, DtoUsuario usuario) =>
{
	try
	{
		await context.Set<Usuario>().AddAsync(new Usuario
        {
            Nombres = usuario.Nombres,
            Correo = usuario.Correo
        });
		await context.SaveChangesAsync();
	}
	catch (Exception ex)
	{
		logger.LogCritical(ex, "Error al insertar {Message}", ex.Message);
	}

    return Results.Ok();
});

app.MapPut("/Usuarios/{id:int}",
    async (ConcurrenciaDbContext context, ILogger<Program> logger, int id, DtoUsuario usuario) =>
    {
        var registro = await context.Set<Usuario>().FindAsync(id);

        try
        {
            if (registro is not null)
            {
                registro.Nombres = usuario.Nombres;
                registro.Correo = usuario.Correo;

                await Task.Delay(TimeSpan.FromSeconds(10));

                await context.SaveChangesAsync();

                return Results.Ok();
            }
        }
        catch (DbUpdateConcurrencyException ex)
        {
            var entry = ex.Entries.Single();
            {
                // Recuperamos el valor actual en la BD
                var actual = await context.Set<Usuario>().AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                if (actual is null)
                    return Results.NotFound();

                foreach (var property in entry.Metadata.GetProperties())
                {
                    logger.LogInformation("Propiedad anterior: {CurrentValue}",
                        context.Entry(actual).Property(property.Name).CurrentValue);
                }
            }
            return Results.Ok(new { Mensaje = "Alguien mas ya edito el registro" });
        }
		catch (Exception ex)
		{
            logger.LogCritical("Error al actualizar {Message}", ex.Message);
        }

        return Results.NotFound();
    });

app.MapGet("/Usuarios", async (ConcurrenciaDbContext context) =>
{
    return Results.Ok(await context.Set<Usuario>().ToListAsync());
});

app.Run();

public record DtoUsuario(string Nombres, string Correo);