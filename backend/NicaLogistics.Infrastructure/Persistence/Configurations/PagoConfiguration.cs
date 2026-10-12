using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Infrastructure.Persistence.Configurations;

public class PagoConfiguration : IEntityTypeConfiguration<Pago>
{
    public void Configure(EntityTypeBuilder<Pago> builder)
    {
        builder.ToTable("pagos");

        builder.HasKey(p => p.IdPago);

        builder.Property(p => p.IdPago)
            .HasColumnName("id_pago")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.IdPedido)
            .HasColumnName("id_pedido")
            .IsRequired();

        builder.Property(p => p.FechaPago)
            .HasColumnName("fecha_pago")
            .IsRequired();

        builder.Property(p => p.Monto)
            .HasColumnName("monto")
            .HasColumnType("numeric(12,2)")
            .IsRequired();

        builder.Property(p => p.Estado)
            .HasColumnName("estado")
            .HasMaxLength(30)
            .IsRequired()
            .HasDefaultValue(Pago.EstadoPendiente);

        builder.Property(p => p.MetodoPago)
            .HasColumnName("metodo_pago")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.FechaReembolso)
            .HasColumnName("fecha_reembolso")
            .IsRequired(false);

        builder.HasIndex(p => p.IdPedido);

        // Un pedido puede tener varios intentos de pago (p. ej. uno cancelado y otro completado)
        builder.HasOne(p => p.Pedido)
            .WithMany()
            .HasForeignKey(p => p.IdPedido)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
