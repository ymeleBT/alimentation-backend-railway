using AlimentationDongmo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlimentationDongmo.Infrastructure.Persistence.Configurations;

public class FournisseurConfiguration : IEntityTypeConfiguration<Fournisseur>
{
    public void Configure(EntityTypeBuilder<Fournisseur> builder)
    {
        builder.ToTable("Fournisseurs");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Nom).IsRequired().HasMaxLength(150);
        builder.Property(f => f.Contact).HasMaxLength(100);
        builder.Property(f => f.Telephone).HasMaxLength(30);
        builder.Property(f => f.Email).HasMaxLength(150);
        builder.Property(f => f.Adresse).HasMaxLength(250);
    }
}
