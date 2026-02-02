using Microsoft.EntityFrameworkCore;
using IncidenciasTI.API.Models;

namespace IncidenciasTI.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<IncidenciaSql> Incidencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IncidenciaSql>(entity =>
            {
                entity.ToTable("Incidencias");

                entity.HasKey(i => i.Id);

                entity.Property(i => i.Titulo)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(i => i.Descripcion)
                      .IsRequired();

                entity.Property(i => i.Estado)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(i => i.Prioridad)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(i => i.FechaCreacion)
                      .IsRequired();
            });
        }
    }
}
