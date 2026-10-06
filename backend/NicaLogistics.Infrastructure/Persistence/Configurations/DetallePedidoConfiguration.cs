using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Infrastructure.Persistence.Configurations;

public class DetallePedidoConfiguration : IEntityTypeConfiguration<DetallePedido>
{
    public void Configure(EntityTypeBuilder<DetallePedido> builder)
    {
        builder.ToTable("detalles_pedido");

        builder.HasKey(d => d.IdDetalle);

        builder.Property(d => d.IdDetalle)
            .HasColumnName("id_detalle")
            .ValueGeneratedOnAdd();

        builder.Property(d => d.IdPedido)
            .HasColumnName("id_pedido")
            .IsRequired();

        builder.Property(d => d.IdProducto)
            .HasColumnName("id_producto")
            .IsRequired();

        builder.Property(d => d.Cantidad)
            .HasColumnName("cantidad")
            .IsRequired();

        builder.Property(d => d.PrecioUnitario)
            .HasColumnName("precio_unitario")
            .HasColumnType("numeric(12,2)")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(d => d.Subtotal)
            .HasColumnName("subtotal")
            .HasColumnType("numeric(12,2)")
            .HasPrecision(12, 2)
            .IsRequired();

        // Relación con Producto
        builder.HasOne(d => d.Producto)
            .WithMany()
            .HasForeignKey(d => d.IdProducto)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con Pedido
        builder.HasOne(d => d.Pedido)
            .WithMany(p => p.Detalles)
            .HasForeignKey(d => d.IdPedido)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
