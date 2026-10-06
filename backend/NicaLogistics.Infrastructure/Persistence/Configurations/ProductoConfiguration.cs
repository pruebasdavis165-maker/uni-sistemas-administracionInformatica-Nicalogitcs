using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Infrastructure.Persistence.Configurations;

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable("productos");

        builder.HasKey(p => p.IdProducto);

        builder.Property(p => p.IdProducto)
            .HasColumnName("id_producto")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Descripcion)
            .HasColumnName("descripcion")
            .HasMaxLength(1000);

        builder.Property(p => p.Precio)
            .HasColumnName("precio")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(p => p.Existencias)
            .HasColumnName("existencias")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.Estado)
            .HasColumnName("estado")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.IdCategoria)
            .HasColumnName("id_categoria")
            .IsRequired();

        // Relación con Categoria
        builder.HasOne(p => p.Categoria)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.IdCategoria)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
