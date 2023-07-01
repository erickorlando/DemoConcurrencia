using ConcurrenciaApi.Tablas;
using Microsoft.EntityFrameworkCore;

namespace ConcurrenciaApi.Data;

public class ConcurrenciaDbContext : DbContext
{
    public ConcurrenciaDbContext(DbContextOptions<ConcurrenciaDbContext> options)
    : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fluent API

        modelBuilder.Entity<Usuario>()
            .Property(p => p.Nombres)
            .HasMaxLength(100);

        modelBuilder.Entity<Usuario>()
            .Property(p => p.Correo)
            .HasMaxLength(200);
        //.IsConcurrencyToken(); // Concurrencia por columna

        modelBuilder.Entity<Usuario>()
            .Property(p => p.Version)
            .IsRowVersion(); // Concurrencia por fila
    }
}