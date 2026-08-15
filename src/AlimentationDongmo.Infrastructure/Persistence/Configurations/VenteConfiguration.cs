using AlimentationDongmo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlimentationDongmo.Infrastructure.Persistence.Configurations;

public class VenteConfiguration : IEntityTypeConfiguration<Vente>
{
    public void Configure(EntityTypeBuilder<Vente> builder)
    {
        builder.ToTable("Ventes");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.MontantTotal).HasColumnType("decimal(12,2)");
        builder.Property(v => v.NumeroTicket).IsRequired().HasMaxLength(30);
        builder.Property(v => v.NomClient).HasMaxLength(150);
        builder.HasIndex(v => v.NumeroTicket).IsUnique();
        builder.HasIndex(v => v.DateHeure);
        builder.HasIndex(v => v.CaissierId);
    }
}

public class LigneVenteConfiguration : IEntityTypeConfiguration<LigneVente>
{
    public void Configure(EntityTypeBuilder<LigneVente> builder)
    {
        builder.ToTable("LignesVente");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.PrixUnitaire).HasColumnType("decimal(12,2)");
        builder.Property(l => l.SousTotal).HasColumnType("decimal(12,2)");

        builder.HasOne(l => l.Vente)
            .WithMany(v => v.Lignes)
            .HasForeignKey(l => l.VenteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.Produit)
            .WithMany(p => p.LignesVente)
            .HasForeignKey(l => l.ProduitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
