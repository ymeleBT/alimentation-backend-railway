using AlimentationDongmo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlimentationDongmo.Infrastructure.Persistence.Configurations;

public class MouvementStockConfiguration : IEntityTypeConfiguration<MouvementStock>
{
    public void Configure(EntityTypeBuilder<MouvementStock> builder)
    {
        builder.ToTable("MouvementsStock");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Reference).HasMaxLength(50);
        builder.Property(m => m.Commentaire).HasMaxLength(250);
        builder.HasIndex(m => m.Date);
        builder.HasIndex(m => new { m.ProduitId, m.Date });

        builder.HasOne(m => m.Produit)
            .WithMany(p => p.MouvementsStock)
            .HasForeignKey(m => m.ProduitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
