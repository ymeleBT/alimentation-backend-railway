using AlimentationDongmo.Domain.Entities;
using AlimentationDongmo.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlimentationDongmo.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Categorie> Categories => Set<Categorie>();
    public DbSet<Produit> Produits => Set<Produit>();
    public DbSet<Fournisseur> Fournisseurs => Set<Fournisseur>();
    public DbSet<ProduitFournisseur> ProduitFournisseurs => Set<ProduitFournisseur>();
    public DbSet<MouvementStock> MouvementsStock => Set<MouvementStock>();
    public DbSet<Vente> Ventes => Set<Vente>();
    public DbSet<LigneVente> LignesVente => Set<LigneVente>();
    public DbSet<CommandeFournisseur> CommandesFournisseur => Set<CommandeFournisseur>();
    public DbSet<LigneCommandeFournisseur> LignesCommandeFournisseur => Set<LigneCommandeFournisseur>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Identity tables renamed to French, without the AspNet prefix, for a cleaner schema
        builder.Entity<ApplicationUser>(b => b.ToTable("Utilisateurs"));
        builder.Entity<IdentityRole<int>>(b => b.ToTable("Roles"));
        builder.Entity<IdentityUserRole<int>>(b => b.ToTable("UtilisateurRoles"));
        builder.Entity<IdentityUserClaim<int>>(b => b.ToTable("UtilisateurClaims"));
        builder.Entity<IdentityUserLogin<int>>(b => b.ToTable("UtilisateurLogins"));
        builder.Entity<IdentityUserToken<int>>(b => b.ToTable("UtilisateurTokens"));
        builder.Entity<IdentityRoleClaim<int>>(b => b.ToTable("RoleClaims"));
    }
}
