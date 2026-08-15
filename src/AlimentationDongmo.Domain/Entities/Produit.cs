namespace AlimentationDongmo.Domain.Entities;

public class Produit
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public int CategorieId { get; set; }
    public Categorie? Categorie { get; set; }
    public string Unite { get; set; } = "unité";
    public decimal PrixAchat { get; set; }
    public decimal PrixVente { get; set; }
    public int QuantiteEnStock { get; set; }
    public int SeuilAlerte { get; set; }
    public string? CodeBarre { get; set; }
    public string? ImageUrl { get; set; }
    public bool Actif { get; set; } = true;
    public DateTime DateCreation { get; set; }

    public ICollection<ProduitFournisseur> ProduitFournisseurs { get; set; } = new List<ProduitFournisseur>();
    public ICollection<MouvementStock> MouvementsStock { get; set; } = new List<MouvementStock>();
    public ICollection<LigneVente> LignesVente { get; set; } = new List<LigneVente>();
    public ICollection<LigneCommandeFournisseur> LignesCommande { get; set; } = new List<LigneCommandeFournisseur>();
}
