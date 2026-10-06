using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Infrastructure.Persistence.Configurations;

public class EnvioConfiguration : IEntityTypeConfiguration<Envio>
{
    public void Configure(EntityTypeBuilder<Envio> builder)
    {
        builder.ToTable("envios");

        builder.HasKey(e => e.IdEnvio);

        builder.Property(e => e.IdEnvio)
            .HasColumnName("id_envio")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.IdPedido)
            .HasColumnName("id_pedido")
            .IsRequired();

        builder.Property(e => e.DireccionDestino)
            .HasColumnName("direccion_destino")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(e => e.EstadoEnvio)
            .HasColumnName("estado_envio")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("EnPreparacion");

        builder.Property(e => e.FechaEstimada)
            .HasColumnName("fecha_estimada")
            .IsRequired(false);

        builder.Property(e => e.GuiaRastreo)
            .HasColumnName("guia_rastreo")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(e => e.GuiaRastreo)
            .IsUnique();

        // Relación 1:1 con Pedido
        builder.HasOne(e => e.Pedido)
            .WithOne(p => p.Envio)
            .HasForeignKey<Envio>(e => e.IdPedido)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
