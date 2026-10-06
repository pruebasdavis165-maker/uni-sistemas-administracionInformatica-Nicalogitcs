using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        // Estrategia TPH (Table Per Hierarchy) para rendimiento óptimo en consultas
        builder.ToTable("usuarios");

        builder.HasKey(u => u.IdUsuario);

        builder.Property(u => u.IdUsuario)
            .HasColumnName("id_usuario")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(u => u.Correo)
            .HasColumnName("correo")
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(u => u.Correo)
            .IsUnique();

        builder.Property(u => u.Contrasena)
            .HasColumnName("contrasena")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.Estado)
            .HasColumnName("estado")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.IdRol)
            .HasColumnName("id_rol")
            .IsRequired();

        // Configuración de Herencia TPH con columna discriminadora
        builder.HasDiscriminator<string>("tipo_usuario")
            .HasValue<Usuario>("Usuario")
            .HasValue<Cliente>("Cliente");

        builder.Property<string>("tipo_usuario")
            .HasMaxLength(50);

        // Relación con Rol
        builder.HasOne(u => u.Rol)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(u => u.IdRol)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
