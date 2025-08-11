namespace ReservaSalones.Infrastructure.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using Domain.Entities; // <- Cambiá según tu namespace

    namespace Infrastructure.Persistence
    {
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

            public DbSet<Salon> Salones { get; set; }

            public DbSet<Reserva> Reservas { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Reserva>(entity =>
                {
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.IdSalon).IsRequired();
                    entity.Property(e => e.Name).IsRequired();

                    entity.HasOne(x => x.Salon).WithMany(s => s.Reservas).HasForeignKey(k => k.IdSalon);
                });

                modelBuilder.Entity<Salon>(entity =>
                {
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.Name).IsRequired().HasMaxLength(500);
                    entity.Property(e => e.Street).IsRequired().HasMaxLength(500);
                    entity.Property(e => e.StreetNumber).IsRequired();
                    entity.Property(e => e.City).IsRequired().HasMaxLength(500);
                    entity.Property(e => e.Town).IsRequired().HasMaxLength(500);
                    entity.Property(e => e.Location).IsRequired().HasMaxLength(1000);
                });

                // Sembrado de datos para la entidad Salon
                modelBuilder.Entity<Salon>().HasData(
                    new Salon
                    {
                        Id = 1,
                        Name = "Salón Principal",
                        Location = "Planta Baja",
                        Street = "Av. Siempre Viva",
                        StreetNumber = 742,
                        Town = "Springfield",
                        City = "Springfield"
                    },
                    new Salon
                    {
                        Id = 2,
                        Name = "Salón de Conferencia",
                        Location = "Primer Piso",
                        Street = "Calle Falsa",
                        StreetNumber = 123,
                        Town = "Springfield",
                        City = "Springfield"
                    },
                    new Salon
                    {
                        Id = 3,
                        Name = "Sala de Reuniones",
                        Location = "Ala Este",
                        Street = "Calle Real",
                        StreetNumber = 456,
                        Town = "Springfield",
                        City = "Springfield"
                    });
            }
        }
    }
}
