using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Infrastructure.Persistence.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("pedidos");

        builder.HasKey(p => p.IdPedido);

        builder.Property(p => p.IdPedido)
            .HasColumnName("id_pedido")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.FechaPedido)
            .HasColumnName("fecha_pedido")
            .IsRequired();

        builder.Property(p => p.Estado)
            .HasColumnName("estado")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Pendiente");

        builder.Property(p => p.Total)
            .HasColumnName("total")
            .HasColumnType("numeric(12,2)")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(p => p.IdCliente)
            .HasColumnName("id_cliente")
            .IsRequired();

        // Relación con Cliente
        builder.HasOne(p => p.Cliente)
            .WithMany()
            .HasForeignKey(p => p.IdCliente)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación 1:N con Detalles
        builder.HasMany(p => p.Detalles)
            .WithOne(d => d.Pedido)
            .HasForeignKey(d => d.IdPedido)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación 1:1 con Envio
        builder.HasOne(p => p.Envio)
            .WithOne(e => e.Pedido)
            .HasForeignKey<Envio>(e => e.IdPedido)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
