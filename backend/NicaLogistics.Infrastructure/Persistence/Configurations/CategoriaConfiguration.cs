using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Infrastructure.Persistence.Configurations;

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("categorias");

        builder.HasKey(c => c.IdCategoria);

        builder.Property(c => c.IdCategoria)
            .HasColumnName("id_categoria")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.NombreCategoria)
            .HasColumnName("nombre_categoria")
            .HasMaxLength(100)
            .IsRequired();

        // Relación 1:N con Producto
        builder.HasMany(c => c.Productos)
            .WithOne(p => p.Categoria)
            .HasForeignKey(p => p.IdCategoria)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
