using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NicaLogistics.Domain.Entities;

namespace NicaLogistics.Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.Property(c => c.Direccion)
            .HasColumnName("direccion")
            .HasMaxLength(300);

        builder.Property(c => c.Telefono)
            .HasColumnName("telefono")
            .HasMaxLength(20);
    }
}
