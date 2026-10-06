using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Infrastructure.Persistence.Configurations;

public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.IdRol);

        builder.Property(r => r.IdRol)
            .HasColumnName("id_rol")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.NombreRol)
            .HasColumnName("nombre_rol")
            .HasMaxLength(100)
            .IsRequired();

        // Relación 1:N con Usuario
        builder.HasMany(r => r.Usuarios)
            .WithOne(u => u.Rol)
            .HasForeignKey(u => u.IdRol)
            .OnDelete(DeleteBehavior.Restrict);

        // Seeder de roles iniciales
        builder.HasData(
            new Rol(1, "Administrador"),
            new Rol(2, "Cliente")
        );
    }
}
