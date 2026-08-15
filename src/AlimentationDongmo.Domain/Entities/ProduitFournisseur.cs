namespace AlimentationDongmo.Domain.Entities;

public class ProduitFournisseur
{
    public int ProduitId { get; set; }
    public Produit? Produit { get; set; }
    public int FournisseurId { get; set; }
    public Fournisseur? Fournisseur { get; set; }
    public decimal PrixAchatFournisseur { get; set; }
}
