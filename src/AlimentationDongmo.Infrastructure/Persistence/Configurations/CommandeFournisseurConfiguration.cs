using AlimentationDongmo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlimentationDongmo.Infrastructure.Persistence.Configurations;

public class CommandeFournisseurConfiguration : IEntityTypeConfiguration<CommandeFournisseur>
{
    public void Configure(EntityTypeBuilder<CommandeFournisseur> builder)
    {
        builder.ToTable("CommandesFournisseur");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.NumeroCommande).IsRequired().HasMaxLength(30);
        builder.HasIndex(c => c.NumeroCommande).IsUnique();
        builder.HasIndex(c => c.DateCommande);

        builder.HasOne(c => c.Fournisseur)
            .WithMany(f => f.Commandes)
            .HasForeignKey(c => c.FournisseurId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class LigneCommandeFournisseurConfiguration : IEntityTypeConfiguration<LigneCommandeFournisseur>
{
    public void Configure(EntityTypeBuilder<LigneCommandeFournisseur> builder)
    {
        builder.ToTable("LignesCommandeFournisseur");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.PrixUnitaire).HasColumnType("decimal(12,2)");

        builder.HasOne(l => l.Commande)
            .WithMany(c => c.Lignes)
            .HasForeignKey(l => l.CommandeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.Produit)
            .WithMany(p => p.LignesCommande)
            .HasForeignKey(l => l.ProduitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
