using AlimentationDongmo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlimentationDongmo.Infrastructure.Persistence.Configurations;

public class ProduitFournisseurConfiguration : IEntityTypeConfiguration<ProduitFournisseur>
{
    public void Configure(EntityTypeBuilder<ProduitFournisseur> builder)
    {
        builder.ToTable("ProduitFournisseurs");
        builder.HasKey(pf => new { pf.ProduitId, pf.FournisseurId });
        builder.Property(pf => pf.PrixAchatFournisseur).HasColumnType("decimal(12,2)");

        builder.HasOne(pf => pf.Produit)
            .WithMany(p => p.ProduitFournisseurs)
            .HasForeignKey(pf => pf.ProduitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pf => pf.Fournisseur)
            .WithMany(f => f.ProduitFournisseurs)
            .HasForeignKey(pf => pf.FournisseurId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
