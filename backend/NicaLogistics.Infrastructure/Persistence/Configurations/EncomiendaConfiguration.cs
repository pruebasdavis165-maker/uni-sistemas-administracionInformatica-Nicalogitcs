using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Infrastructure.Persistence.Configurations;

public class EncomiendaConfiguration : IEntityTypeConfiguration<Encomienda>
{
    public void Configure(EntityTypeBuilder<Encomienda> builder)
    {
        builder.ToTable("encomiendas");

        builder.HasKey(e => e.IdEncomienda);

        builder.Property(e => e.IdEncomienda)
            .HasColumnName("id_encomienda")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.IdPedido)
            .HasColumnName("id_pedido")
            .IsRequired();

        builder.Property(e => e.Origen)
            .HasColumnName("origen")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(e => e.Destino)
            .HasColumnName("destino")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(e => e.Estado)
            .HasColumnName("estado")
            .HasMaxLength(30)
            .IsRequired()
            .HasDefaultValue(Encomienda.EstadoRegistrada);

        builder.Property(e => e.FechaRegistro)
            .HasColumnName("fecha_registro")
            .IsRequired();

        // Una encomienda por pedido: garantiza que la automatización sea idempotente
        builder.HasIndex(e => e.IdPedido).IsUnique();

        builder.HasOne(e => e.Pedido)
            .WithMany()
            .HasForeignKey(e => e.IdPedido)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
