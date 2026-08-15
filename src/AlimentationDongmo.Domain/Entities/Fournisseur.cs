namespace AlimentationDongmo.Domain.Entities;

public class Fournisseur
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Contact { get; set; }
    public string? Telephone { get; set; }
    public string? Email { get; set; }
    public string? Adresse { get; set; }
    public bool Actif { get; set; } = true;

    public ICollection<ProduitFournisseur> ProduitFournisseurs { get; set; } = new List<ProduitFournisseur>();
    public ICollection<CommandeFournisseur> Commandes { get; set; } = new List<CommandeFournisseur>();
}
