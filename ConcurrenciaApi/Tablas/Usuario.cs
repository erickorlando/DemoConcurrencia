namespace ConcurrenciaApi.Tablas;

public class Usuario
{
    public int Id { get; set; }
    public string Nombres { get; set; } = null!;

    //[ConcurrencyCheck] // DataAnnotations
    public string Correo { get; set; } = null!;

    public byte[] Version { get; set; }
}