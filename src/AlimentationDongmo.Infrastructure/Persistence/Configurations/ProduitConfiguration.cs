using AlimentationDongmo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlimentationDongmo.Infrastructure.Persistence.Configurations;

public class ProduitConfiguration : IEntityTypeConfiguration<Produit>
{
    public void Configure(EntityTypeBuilder<Produit> builder)
    {
        builder.ToTable("Produits");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Nom).IsRequired().HasMaxLength(150);
        builder.Property(p => p.Unite).IsRequired().HasMaxLength(30);
        builder.Property(p => p.PrixAchat).HasColumnType("decimal(12,2)");
        builder.Property(p => p.PrixVente).HasColumnType("decimal(12,2)");
        builder.Property(p => p.CodeBarre).HasMaxLength(50);
        builder.Property(p => p.ImageUrl).HasMaxLength(300);
        builder.HasIndex(p => p.CodeBarre).IsUnique();

        builder.HasOne(p => p.Categorie)
            .WithMany(c => c.Produits)
            .HasForeignKey(p => p.CategorieId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
